# Student Management System — Learnings & Technical Reference

> **Deep-dive documentation** covering every file, code approach, architecture, logic flow, and design decision in this project.  
> Last updated: April 19, 2026

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [Technology Stack](#2-technology-stack)
3. [Project Structure](#3-project-structure)
4. [Architecture Pattern](#4-architecture-pattern)
5. [Database Layer](#5-database-layer)
6. [Data Models (Entities)](#6-data-models-entities)
7. [Repository Layer](#7-repository-layer)
8. [Service Layer](#8-service-layer)
9. [Authentication & Authorization](#9-authentication--authorization)
10. [Helper Utilities](#10-helper-utilities)
11. [Layouts](#11-layouts)
12. [Shared Components](#12-shared-components)
13. [Application Pages](#13-application-pages)
14. [Routing & Navigation](#14-routing--navigation)
15. [Styling & Theming](#15-styling--theming)
16. [Configuration Files](#16-configuration-files)
17. [Data Flow Diagrams](#17-data-flow-diagrams)
18. [Common Gotchas & Lessons Learned](#18-common-gotchas--lessons-learned)

---

## 1. System Overview

The **Student Management System** is a server-side Blazor (.NET 9) web application that allows:

- **Students** — to view courses, enrollment charts, and their own grades.
- **Instructors** — full CRUD on students, courses, enrollments, and grades.
- **Admins** — everything Instructors can do, plus a **User Management** page (CRUD on all user accounts).

The system uses **MudBlazor** for UI components, **Entity Framework Core** with **MySQL** (Pomelo provider) for data persistence, and a **custom claims-based authentication** system (no ASP.NET Identity).

---

## 2. Technology Stack

| Technology | Version | Purpose |
|---|---|---|
| .NET | 9.0 | Runtime and framework |
| Blazor Server | Interactive SSR | Real-time UI with SignalR |
| MudBlazor | 9.2.0 | Material Design UI components |
| Entity Framework Core | 9.0.5 | ORM — database access |
| Pomelo.EntityFrameworkCore.MySql | 9.0.0 | MySQL provider for EF Core |
| MySQL | 8.0.36 | Relational database |

**File**: `StudentManagementSystem.csproj` (Lines 1–18)
- `<TargetFramework>net9.0</TargetFramework>` — targets .NET 9
- All NuGet packages declared in `<ItemGroup>` — EF Core, MudBlazor, Pomelo MySQL

---

## 3. Project Structure

```
StudentManagementSystem/
├── Components/                    # All Blazor UI components
│   ├── App.razor                  # Root HTML document (head, body, scripts)
│   ├── Routes.razor               # Router + AuthorizeRouteView + ErrorBoundary
│   ├── _Imports.razor             # Global @using directives
│   ├── Layout/                    # Page layouts
│   │   ├── MainLayout.razor       # Authenticated layout (sidebar + appbar)
│   │   ├── EmptyLayout.razor      # Login/Register layout (centered card)
│   │   ├── LandingLayout.razor    # Landing page layout (no sidebar)
│   │   └── NavMenu.razor          # Role-based sidebar navigation
│   ├── Pages/                     # All routable pages
│   │   ├── Home.razor             # Landing page (/)
│   │   ├── Login.razor            # Login page (/login)
│   │   ├── Register.razor         # Multi-step registration (/register)
│   │   ├── Dashboard.razor        # Dashboard with stats (/dashboard)
│   │   ├── Logout.razor           # Logout confirmation (/logout)
│   │   ├── StudentManagement.razor    # CRUD for students (/students)
│   │   ├── CourseManagement.razor     # CRUD for courses (/courses)
│   │   ├── EnrollmentManagement.razor # CRUD for enrollments (/enrollments)
│   │   ├── GradeManagement.razor      # CRUD for grades (/grades)
│   │   ├── UserManagement.razor       # Admin CRUD for users (/users)
│   │   ├── EnrollmentChartTable.razor # Student enrollment view
│   │   ├── MyGrade.razor              # Student grade view (/my-grades)
│   │   ├── NotFound.razor             # 404 page
│   │   └── Error.razor                # Error page
│   └── Shared/                    # Reusable UI components
│       ├── FloatingSuccessModal.razor  # Success modal popup
│       └── PasswordField.razor        # Password input with toggle visibility
├── Features/                      # Backend logic (Clean Architecture)
│   ├── Data/                      # Database layer
│   │   ├── AppDbContext.cs        # EF Core DbContext
│   │   ├── Enums/
│   │   │   └── UserRole.cs        # Student, Instructor, Admin enum
│   │   └── Models/                # Entity models
│   │       ├── Student.cs
│   │       ├── Course.cs
│   │       ├── Enrollment.cs
│   │       ├── Grade.cs
│   │       └── User.cs
│   ├── Models/                    # View/Request models
│   │   └── RegisterRequest.cs     # Registration form data
│   ├── Repositories/              # Data access layer
│   │   ├── Interfaces/            # Contracts
│   │   └── Implementations/       # EF Core implementations
│   ├── Services/                  # Business logic layer
│   │   ├── Interfaces/            # Contracts
│   │   └── Implementations/       # Service implementations
│   └── Helpers/                   # Utility classes
│       ├── CustomAuthenticationStateProvider.cs  # Auth state management
│       ├── PersonNameHelper.cs                  # Name + honorific builder
│       └── RedirectToLogin.cs                   # Unauthorized redirect
├── Migrations/                    # EF Core database migrations
├── wwwroot/                       # Static web assets
│   ├── app.css                    # Global styles + sidebar active states
│   └── favicon.png                # App icon
├── Program.cs                     # Application entry point & DI setup
├── appsettings.json               # Connection strings & logging config
└── Properties/
    └── launchSettings.json        # HTTPS/HTTP port configuration
```

---

## 4. Architecture Pattern

This project follows a **layered architecture** (Repository Pattern + Service Layer):

```
┌──────────────────────────────┐
│        Blazor UI Pages       │  ← Components/Pages/*.razor
│     (Presentation Layer)     │
└──────────┬───────────────────┘
           │ @inject IXxxService
┌──────────▼───────────────────┐
│       Service Layer          │  ← Features/Services/Implementations/*.cs
│     (Business Logic)         │
└──────────┬───────────────────┘
           │ IXxxRepository
┌──────────▼───────────────────┐
│     Repository Layer         │  ← Features/Repositories/Implementations/*.cs
│     (Data Access)            │
└──────────┬───────────────────┘
           │ AppDbContext (EF Core)
┌──────────▼───────────────────┐
│     MySQL Database           │  ← StudentManagementDB
└──────────────────────────────┘
```

**Why this pattern?**
- **Separation of Concerns**: UI → Service → Repository → Database. Pages never touch EF Core directly.
- **Testability**: Interfaces (`IXxxService`, `IXxxRepository`) allow mocking for unit tests.
- **Maintainability**: Business rules live in services, data access in repositories, UI in components.

**Dependency Injection Registration** — `Program.cs` (Lines 26–38):
```csharp
// Repositories — registered as Scoped (one instance per SignalR circuit)
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
builder.Services.AddScoped<IGradeRepository, GradeRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services — call repositories, contain business logic
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IGradeService, GradeService>();
builder.Services.AddScoped<IUserService, UserService>();
```

---

## 5. Database Layer

### `Features/Data/AppDbContext.cs` (Lines 1–69)

**Purpose**: Central EF Core database context that maps C# entity classes to MySQL tables.

**Key Code**:
- **Lines 10–14**: Five `DbSet<T>` properties — one per table:
  - `Students`, `Courses`, `Enrollments`, `Grades`, `Users`
- **Lines 16–67**: `OnModelCreating()` — configures entity relationships:

| Relationship | Line | FK | Delete Behavior |
|---|---|---|---|
| Student → Course | 21–25 | `Student.CourseId` | Restrict (can't delete course with students) |
| Enrollment → Student | 28–32 | `Enrollment.StudentId` | Cascade (delete student = delete enrollments) |
| Enrollment → Course | 35–39 | `Enrollment.CourseId` | Cascade |
| Grade → Enrollment | 42–46 | `Grade.EnrollmentId` | Cascade (one-to-one) |
| User → Student | 49–53 | `User.StudentId` | SetNull (delete student = null out user's link) |

- **Lines 56–66**: Three unique indexes:
  - `Student.StudentNumber` — no duplicate student numbers
  - `Course.CourseCode` — no duplicate course codes
  - `User.Username` — no duplicate usernames

**Connected files**: All Repository implementations use `AppDbContext` via `IServiceScopeFactory`.

**Why `IServiceScopeFactory`?** — Blazor Server circuits are long-lived. If we inject `AppDbContext` directly, EF Core's change tracker accumulates stale data across multiple user actions. By creating a **new scope per operation**, each database call gets a fresh `DbContext` with no tracking conflicts.

---

## 6. Data Models (Entities)

### `Features/Data/Models/Student.cs` (Lines 1–28)

**Purpose**: Represents a student record in the `Students` table.

| Property | Type | Purpose |
|---|---|---|
| `Id` | `int` | Primary key (auto-increment) |
| `StudentNumber` | `string` | Auto-generated unique ID (format: `STU-yyyyMMddHHmmssfff`) |
| `FullName` | `string` | Pre-computed full name stored in DB |
| `FirstName`, `MiddleName`, `Surname`, `Suffix` | `string` | Name parts |
| `Gender`, `CivilStatus` | `string` | Personal info |
| `Address`, `Email` | `string` | Contact info |
| `Department`, `Program` | `string` | Academic info |
| `InstructorName` | `string` | Assigned instructor |
| `CourseId` | `int?` | FK → Course (nullable, major/program) |
| `YearLevel` | `int` | 1st to 4th year |
| `CreatedAt` | `DateTime` | Auto-set to `DateTime.UtcNow` |

**Navigation properties** (Lines 24–26):
- `Course?` — the student's major/program course
- `User?` — the linked login account (one-to-one)
- `Enrollments` — all course enrollments (one-to-many)

---

### `Features/Data/Models/Course.cs` (Lines 1–15)

**Purpose**: Represents a course offering.

| Property | Type | Purpose |
|---|---|---|
| `Id` | `int` | Primary key |
| `CourseCode` | `string` | Unique code (e.g., "CS101") |
| `CourseName` | `string` | Full course name |
| `Description` | `string` | Course description |
| `Units` | `int` | Credit units |

**Navigation**: `Students` (students majoring in this course), `Enrollments` (enrollments in this course)

---

### `Features/Data/Models/Enrollment.cs` (Lines 1–17)

**Purpose**: Links a student to a course for a specific semester.

| Property | Type | Purpose |
|---|---|---|
| `StudentId` | `int` | FK → Student |
| `CourseId` | `int` | FK → Course |
| `Semester` | `string` | e.g., "1st Semester 2026" |
| `EnrollmentDate` | `DateTime` | When enrolled |
| `Status` | `string` | Default: "Enrolled" |

**Navigation**: `Student`, `Course`, `Grade` (one-to-one)

---

### `Features/Data/Models/Grade.cs` (Lines 1–15)

**Purpose**: Academic grade for one enrollment (one-to-one with Enrollment).

| Property | Type | Purpose |
|---|---|---|
| `EnrollmentId` | `int` | FK → Enrollment |
| `GradeValue` | `decimal` | Numeric grade (e.g., 1.25) |
| `LetterGrade` | `string` | Letter grade (e.g., "A") |
| `Remarks` | `string` | Pass/Fail/INC |
| `GradedAt` | `DateTime` | When graded |

---

### `Features/Data/Models/User.cs` (Lines 1–31)

**Purpose**: Login account. Can be linked to a Student record for Student-role users.

| Property | Type | Purpose |
|---|---|---|
| `Username` | `string` | Unique login username |
| `PasswordHash` | `string` | SHA-256 hash of password |
| `Role` | `UserRole` | Enum: Student / Instructor / Admin |
| `StudentId` | `int?` | FK → Student (null for Instructor/Admin) |
| `FirstName`, `MiddleName`, `Surname`, `Suffix` | `string` | Personal info |
| `Gender`, `CivilStatus` | `string` | Used for honorific logic (Mr./Ms./Mrs.) |
| `Email`, `Address`, `MajorProfession` | `string` | Contact and academic info |

**Line 27–29** — `[NotMapped] FullName` — computed property that joins name parts. Not stored in DB.

**Key Design Decision**: Admin and Instructor users do NOT create Student records — `StudentId` is null for them.

---

### `Features/Data/Enums/UserRole.cs` (Lines 1–9)

```csharp
public enum UserRole
{
    Instructor,  // value = 0
    Student,     // value = 1
    Admin        // value = 2
}
```

**Why int-backed enum?** — Stored as integer in MySQL. The order matters for database values.

---

## 7. Repository Layer

**Pattern**: Each repository creates a **new `IServiceScope`** per method call via `IServiceScopeFactory`. This prevents EF Core tracking conflicts in long-lived Blazor circuits.

### `IUserRepository` → `UserRepository`

**File**: `Features/Repositories/Implementations/UserRepository.cs` (Lines 1–127)

| Method | Line | Purpose |
|---|---|---|
| `GetByUsernameAsync(string)` | 17–26 | Find user by username (with `.Include(u => u.Student)`), `AsNoTracking()` |
| `AddAsync(User)` | 28–36 | Insert new user |
| `GetByIdAsync(int)` | 38–47 | Find user by ID, `AsNoTracking()` |
| `GetByStudentIdAsync(int)` | 49–58 | Find user by linked student |
| `UpdateAsync(User)` | 60–87 | **Explicit property-by-property copy** from detached entity to tracked entity |
| `DeleteByStudentIdAsync(int)` | 89–100 | Delete user by student link |
| `GetAllAsync()` | 102–112 | All users, ordered by `CreatedAt DESC` |
| `DeleteAsync(int)` | 114–125 | Delete user by ID |

**Critical Design Decision** (Line 60–87): `UpdateAsync` uses **explicit property assignment** instead of `SetValues()`:
```csharp
existingUser.Username = user.Username;
existingUser.PasswordHash = user.PasswordHash;
// ... all 13 properties
```
**Why?** — `SetValues()` was silently ignoring some property changes (like Username) when copying from a detached entity with navigation properties. Explicit assignment guarantees every field is persisted. This is critical for the "edit credentials → login with new credentials" flow.

---

### Other Repositories (Same Pattern)

| Repository | File | Key Notes |
|---|---|---|
| `StudentRepository` | `StudentRepository.cs` | `.Include(s => s.Course)` on reads; explicit property copy on update |
| `CourseRepository` | `CourseRepository.cs` | Standard CRUD |
| `EnrollmentRepository` | `EnrollmentRepository.cs` | `.Include(e => e.Student).Include(e => e.Course).Include(e => e.Grade)` — deep includes for display |
| `GradeRepository` | `GradeRepository.cs` | `.Include(g => g.Enrollment).ThenInclude(e => e.Student/Course)` — deep navigation |

---

## 8. Service Layer

Services sit between UI and repositories. They contain **business logic** and **cross-cutting concerns**.

### `UserService` — `Features/Services/Implementations/UserService.cs` (Lines 1–129)

**The most complex service.** Handles authentication, registration, and user management.

| Method | Lines | Logic |
|---|---|---|
| `ValidateLoginAsync` | 23–31 | (1) Find user by username, (2) Check role matches, (3) Hash input password with SHA-256, (4) Compare hash with stored hash. Returns `null` on mismatch. |
| `RegisterAsync` | 33–79 | (1) Check username uniqueness, (2) If Student role → create `Student` record first with auto-generated `StudentNumber`, (3) Create `User` record linking to student. Admin/Instructor skip student creation. |
| `UpdateUserWithPasswordAsync` | 96–100 | Hash new password, then delegate to `UpdateAsync`. This ensures edited credentials allow login. |
| `DeleteUserAsync` | 102–115 | If user has a linked Student, delete student first, then delete user. |
| `HashPassword` | 117–122 | SHA-256 hash → Base64 string. Private static method. |
| `GenerateStudentNumber` | 124–127 | Format: `STU-yyyyMMddHHmmssfff` — timestamp-based unique number. |

**Password Flow** — How login works after editing credentials:
```
Edit User → UpdateUserWithPasswordAsync(user, "newpass")
  → user.PasswordHash = SHA256("newpass") → Base64
  → Repository.UpdateAsync(user)
    → existingUser.PasswordHash = user.PasswordHash  // explicit copy
    → SaveChangesAsync()  // UPDATE Users SET PasswordHash = @p0 WHERE Id = @p1

Login → ValidateLoginAsync("username", "newpass", role)
  → SHA256("newpass") → Base64  // same hash
  → user.PasswordHash == hash  → true → login success
```

---

## 9. Authentication & Authorization

This system uses **custom claims-based authentication** — NOT ASP.NET Identity. No cookies, no tokens, no database sessions. State is held **in-memory** per Blazor circuit.

### `CustomAuthenticationStateProvider.cs` (Lines 1–71)

**Purpose**: Manages the current user's authentication state in the Blazor circuit.

| Method | Lines | Purpose |
|---|---|---|
| `GetAuthenticationStateAsync()` | 12–16 | Returns current `ClaimsPrincipal`. Anonymous if not logged in. |
| `MarkUserAsAuthenticated(User)` | 18–34 | Creates claims: `Name`, `Role`, `UserId`, `StudentId`. Notifies all `<AuthorizeView>` components. |
| `MarkUserAsLoggedOut()` | 37–42 | Clears claims → sets anonymous principal. |
| `GetCurrentUsername()` | 44–47 | Reads `Name` claim. |
| `GetCurrentRole()` | 49–52 | Reads `Role` claim. |
| `GetCurrentUserId()` | 54–58 | Reads custom `UserId` claim. |
| `GetCurrentStudentId()` | 60–64 | Reads custom `StudentId` claim (null for non-students). |
| `IsInRole(UserRole)` | 66–69 | Check if current user has a specific role. |

**Claims created on login** (Lines 20–28):
```csharp
new Claim(ClaimTypes.Name, user.Username),      // "admin1"
new Claim(ClaimTypes.Role, user.Role.ToString()), // "Admin"
new Claim("UserId", user.Id.ToString()),          // "5"
new Claim("StudentId", ...)                       // only for Students
```

**Why custom auth?** — Simpler than Identity for a school project. No cookie/token management. The tradeoff is that auth state is lost on page refresh (Blazor circuit restart).

### `RedirectToLogin.cs` (Lines 1–35)

**Purpose**: A component that redirects unauthenticated users to `/login?returnUrl=...`.

**Used in**: `Routes.razor` Line 10 — `<NotAuthorized><RedirectToLogin /></NotAuthorized>`

**Flow**: When an unauthorized user hits a protected page → `AuthorizeRouteView` renders `<NotAuthorized>` → `RedirectToLogin` captures the current URL → navigates to `/login?returnUrl=<original-url>`.

### Registration in `Program.cs` (Lines 40–45):
```csharp
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());
```
**Why the double registration?** — `CustomAuthenticationStateProvider` is registered first as itself (so pages can inject it directly for `MarkUserAsAuthenticated`), then aliased to `AuthenticationStateProvider` (so Blazor's built-in auth system uses it).

---

## 10. Helper Utilities

### `PersonNameHelper.cs` (Lines 1–25)

| Method | Lines | Purpose |
|---|---|---|
| `BuildFullName(first, middle, surname, suffix)` | 5–9 | Joins non-empty name parts with spaces |
| `BuildInstructorHonorific(gender, civilStatus)` | 11–23 | Returns "Mr", "Ms", or "Mrs" |

**Honorific Logic** (Lines 13–22):
- Male (any civil status) → **Mr**
- Female + Single → **Ms**
- Female + Married → **Mrs**
- Female + any other status → **Ms** (default)

**Used in**: Login page welcome modal (Instructor greeting), Student record creation.

---

## 11. Layouts

### `MainLayout.razor` (Lines 1–54) — Authenticated Pages

**Structure**: `MudLayout` → `MudDrawer` (sidebar) + `MudMainContent` (page area)

- **Line 9**: `MudDrawer` with persistent drawer variant — stays open by default
- **Line 10**: Dark navy sidebar (`#1A237E`)
- **Line 15**: `<NavMenu />` — role-based navigation
- **Line 20**: `MudAppBar` — top bar with hamburger menu
- **Line 24**: `@Body` — rendered page content with 24px padding

**Theme** (Lines 32–47): Custom MudBlazor theme:
- Primary: `#1A237E` (dark navy blue)
- Secondary: `#E3F2FD` (light blue)
- Background: `#F8F9FA` (light gray)
- Drawer colors: navy background, white text

### `EmptyLayout.razor` (Lines 1–32) — Login/Register

**Purpose**: Centered card layout with no sidebar. Used for unauthenticated pages.

- **Line 8**: Full-viewport flex container, centered both axes
- **Line 9**: Max-width 480px card area
- **Lines 10–13**: "Student Management System" title header above the form

### `LandingLayout.razor` (Lines 1–26) — Home Page

**Purpose**: Minimal layout — just the page content on a light gray background. No sidebar, no appbar.

### `NavMenu.razor` (Lines 1–63)

**Purpose**: Role-based sidebar navigation. Wrapped in `<AuthorizeView>` — only shows links when authenticated.

**Role routing** (Lines 10–57):

| Role | Visible Nav Links |
|---|---|
| **Admin** | Dashboard, Students, Courses, Enrollments, Grades, **Users**, Logout |
| **Instructor** | Dashboard, Students, Courses, Enrollments, Grades, Logout |
| **Student** | Dashboard, Courses, Enrollment Chart Table, My Grade, Logout |

- **Line 56**: Logout link styled in orange (`#FF9800`) for visual distinction

---

## 12. Shared Components

### `FloatingSuccessModal.razor` (Lines 1–25)

**Purpose**: Reusable success popup with checkmark icon, title, message, and "Continue" button.

**Parameters**:
| Parameter | Type | Default | Purpose |
|---|---|---|---|
| `Visible` | `bool` | — | Show/hide the dialog |
| `VisibleChanged` | `EventCallback<bool>` | — | Two-way binding for visibility |
| `Title` | `string` | `"Successfully Created"` | Modal heading (customizable for login "Welcome!") |
| `Message` | `string` | `""` | Body text (e.g., "Welcome Admin Test!") |

**Used in**: Register.razor (account creation), Login.razor (welcome modal), UserManagement.razor (CRUD feedback)

**Critical Blazor Rule**: When passing C# variables as parameter values, you MUST use the `@` prefix:
```razor
✅ Message="@_successMessage"    ← passes variable value
❌ Message="_successMessage"     ← passes literal string "_successMessage"
```

### `PasswordField.razor` (Lines 1–30)

**Purpose**: Password input with visibility toggle (eye icon).

**Parameters**: `Value`, `ValueChanged`, `Label`, `Required`

**Key approach** (Lines 2–10):
- Uses `MudTextField` with `InputType` that toggles between `InputType.Password` and `InputType.Text`
- `Adornment.End` with eye/eye-off icon for toggle
- `Immediate="true"` — triggers `ValueChanged` on every keystroke (not just on blur)

**The `@` prefix rule applies here too**:
```razor
✅ Value="@_password"       ← empty string → empty field
❌ Value="_password"        ← literal "password" → 9 dots showing
```

---

## 13. Application Pages

### `Login.razor` (Lines 1–126) — Route: `/login`

**Layout**: `EmptyLayout` (centered card, no sidebar)

**UI Flow**:
1. Role dropdown (Student / Instructor / Admin)
2. Username text field
3. Password field (PasswordField component)
4. "Sign In" button (disabled until all fields filled)

**Login Logic** (Lines 94–122):
1. Call `UserService.ValidateLoginAsync(username, password, role)`
2. If user found → `AuthProvider.MarkUserAsAuthenticated(user)` — sets claims
3. Build role-based welcome message:
   - Student: `"Welcome {FirstName}!"`
   - Instructor: `"Welcome {Mr/Ms/Mrs}. {Surname}!"` (via `PersonNameHelper.BuildInstructorHonorific`)
   - Admin: `"Welcome Admin {FirstName}!"`
4. Show `FloatingSuccessModal` with title "Welcome!"
5. On "Continue" click → navigate to `/dashboard`

**Related files**: `IUserService`, `UserService`, `CustomAuthenticationStateProvider`, `PersonNameHelper`, `FloatingSuccessModal`, `PasswordField`

---

### `Register.razor` (Lines 1–361) — Route: `/register`

**Layout**: `EmptyLayout`

**Multi-step registration** — step count depends on role:
- **Student/Instructor**: 4 steps
- **Admin**: 3 steps (no email/address/major step)

| Step | Student/Instructor | Admin |
|---|---|---|
| 1 | Role selection | Role selection |
| 2 | Personal info (name, gender, civil status) | Personal info (name, gender, civil status) |
| 3 | Email, address, major/department | Username + Password + Confirm |
| 4 | Username + Password + Confirm | *(done)* |

**Button validation** — "Next" button disabled until all required fields filled:
- `CanMoveNext` computed property checks current step's required fields
- `CanCreate` checks username, password, and password match

**Registration Flow** (final step):
1. Build `RegisterRequest` model from form fields
2. Call `UserService.RegisterAsync(request)`
3. Show `FloatingSuccessModal`: "Welcome Admin {FirstName}!" or "Welcome {FirstName}!"
4. On "Continue" → navigate to `/login`

---

### `Dashboard.razor` (Lines 1–137) — Route: `/dashboard`

**Purpose**: Overview page with stat cards and role-based quick action buttons.

**Stats Cards** (Lines 28–52): Four `MudPaper` cards showing counts:
- Students, Courses, Enrollments, Grades

**Quick Actions** (Lines 54–96):
- Admin: Students, Courses, Enrollments, Grades, **Users** buttons
- Instructor: Students, Courses, Enrollments, Grades buttons
- Student: No quick action buttons

**Data loading** (Lines 107–135): Calls all four services in `OnInitializedAsync()` to get counts.

---

### `UserManagement.razor` (Lines 1–471) — Route: `/users`, `/usermanagement`

**Purpose**: Admin-only CRUD for all user accounts. The most complex page.

**Access Control** (Lines 15–25): Checks auth state → if not Admin, shows warning alert.

**Table** (Lines 33–80): `MudTable<User>` with columns: Username, Full Name, Role, Email, Gender, Actions.

**CRUD Dialogs**:

| Dialog | Lines | Features |
|---|---|---|
| **Add User** | 85–142 | All fields + role dropdown + password + confirm password |
| **View User** | *(read-only details)* | Modal with all user info |
| **Edit User** | 144–181 | All fields + username + **optional password change** |
| **Delete User** | 184–202 | Confirmation dialog: "Are you sure?" |

**Edit with Password Change** (Lines 397–418):
- If new password is provided → call `UpdateUserWithPasswordAsync(user, newPassword)` — hashes and saves
- If password left blank → call `UpdateUserAsync(user)` — keeps existing hash
- Both paths update all fields including Username via explicit property assignment

---

### `StudentManagement.razor` — Route: `/students`

**Access**: Instructor or Admin only.

**CRUD**: Add, View, Edit, Delete students via modal dialogs. Auto-generates student number on add.

---

### `CourseManagement.razor` — Route: `/courses`

**Access**: Instructor/Admin for CRUD buttons; Students can view course list.

---

### `EnrollmentManagement.razor` — Route: `/enrollments`

**Access**: Instructor or Admin.

**Feature**: Enroll a student in a course with semester and date selection.

---

### `GradeManagement.razor` — Route: `/grades`

**Access**: Instructor or Admin.

**Feature**: Assign grades to enrollments with numeric value, letter grade, and remarks.

---

### `Logout.razor` (Lines 1–44) — Route: `/logout`

**Purpose**: Confirmation dialog asking "Do you want to Logout?"

**Buttons**:
- **Cancel** → navigate back to `/dashboard`
- **Logout** → `AuthProvider.MarkUserAsLoggedOut()` → navigate to `/` (home)

**Dialog Options** (Lines 24–29): `CloseOnEscapeKey = false`, `BackdropClick = false` — forces user to choose.

---

### `Home.razor` (Lines 1–45) — Route: `/`, `/home`

**Layout**: `LandingLayout`

**Purpose**: Public landing page with "Sign In" and "Register" buttons, plus four feature cards.

---

### `MyGrade.razor` — Route: `/my-grades`

**Access**: Student only. Displays the logged-in student's grades by looking up their `StudentId` claim.

### `EnrollmentChartTable.razor` — Route: `/enrollment-chart-table`

**Access**: Student only. Shows the student's enrollment records.

---

## 14. Routing & Navigation

### `App.razor` (Lines 1–26)

**Purpose**: Root HTML document. Contains `<head>` (stylesheets, fonts, MudBlazor CSS), `<body>` (Blazor app).

**Key Lines**:
- **Line 8**: Google Fonts — Roboto for Material Design feel
- **Line 9**: MudBlazor CSS — `_content/MudBlazor/MudBlazor.min.css`
- **Line 18**: `<CascadingAuthenticationState>` wraps entire app — makes auth state available everywhere
- **Line 19**: `<Routes @rendermode="InteractiveServer" />` — enables interactive server mode
- **Line 21**: MudBlazor JS — `_content/MudBlazor/MudBlazor.min.js`

### `Routes.razor` (Lines 1–57)

**Purpose**: Application router with auth-aware routing.

**Key Structure**:
```
Router
├── Found → ErrorBoundary
│   ├── AuthorizeRouteView (default layout: MainLayout)
│   │   ├── NotAuthorized → RedirectToLogin
│   │   └── Authorizing → MudProgressCircular
│   └── ErrorContent → MudAlert + "Return Home" button
└── NotFound → NotFound component
```

**Error Recovery** (Lines 39–55): Subscribes to `Navigation.LocationChanged` — clears error boundary on navigation. Prevents "stuck on error" state.

### `_Imports.razor` (Lines 1–21)

**Purpose**: Global `@using` directives available to all `.razor` files. Eliminates repetitive imports.

Key namespaces imported:
- `MudBlazor` — UI components
- `Microsoft.AspNetCore.Components.Authorization` — `<AuthorizeView>`, etc.
- `StudentManagementSystem.Components.Shared` — `FloatingSuccessModal`, `PasswordField`
- `StudentManagementSystem.Features.Data.Models` — `Student`, `Course`, `User`, etc.
- `StudentManagementSystem.Features.Data.Enums` — `UserRole`

---

## 15. Styling & Theming

### Color Palette

| Color | Hex | Usage |
|---|---|---|
| Dark Navy | `#1A237E` | Primary brand color, sidebar, appbar, headings |
| Light Blue | `#E3F2FD` | Secondary accents |
| Orange | `#FF9800` | Active sidebar indicator, logout link, accent borders |
| Gray | `#546E7A` | Body text, descriptions |
| Light Gray | `#F8F9FA` | Page background |
| White | `#FFFFFF` | Surface/card backgrounds, nav text |

### `wwwroot/app.css` — Sidebar Active States (Lines 63–85)

```css
/* Active nav link — clearly visible against dark navy */
.mud-nav-link.active {
    background-color: rgba(255, 255, 255, 0.2) !important;
    border-left: 4px solid #FF9800 !important;
    font-weight: 700 !important;
}
.mud-nav-link.active .mud-nav-link-text {
    color: #FFFFFF !important;
    font-weight: 700 !important;
}
.mud-nav-link.active .mud-icon-root {
    color: #FF9800 !important;
}

/* Hover effect */
.mud-nav-link:hover {
    background-color: rgba(255, 255, 255, 0.1) !important;
}
```

**Why `!important`?** — MudBlazor applies its own inline styles and scoped CSS. `!important` ensures our custom styles override MudBlazor defaults on the dark sidebar.

---

## 16. Configuration Files

### `appsettings.json` (Lines 1–13)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=StudentManagementDB;User=root;Password=;"
  }
}
```

**MySQL connection**: localhost on port 3306, database `StudentManagementDB`, user `root`, no password.

### `Program.cs` — Auto Migration (Lines 56–60)

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();  // Apply pending migrations on startup
}
```

**Why?** — Ensures the database schema is always up-to-date when the app starts. No manual `dotnet ef database update` needed.

---

## 17. Data Flow Diagrams

### Login Flow
```
User enters credentials → Login.razor
  → HandleLogin() calls UserService.ValidateLoginAsync()
    → UserRepository.GetByUsernameAsync() → MySQL SELECT
    → HashPassword(input) → compare with user.PasswordHash
  ← returns User object (or null)
  → CustomAuthenticationStateProvider.MarkUserAsAuthenticated(user)
    → creates ClaimsPrincipal with Name, Role, UserId claims
    → NotifyAuthenticationStateChanged()
      → all <AuthorizeView> components re-render
  → Show FloatingSuccessModal("Welcome!")
  → User clicks Continue → NavigateTo("/dashboard")
```

### Registration Flow (Student)
```
Register.razor → Step 1 (Role) → Step 2 (Personal) → Step 3 (Contact) → Step 4 (Credentials)
  → HandleSubmit() creates RegisterRequest
    → UserService.RegisterAsync(request)
      → Check username uniqueness
      → Create Student record first (auto StudentNumber)
      → Create User record (linked to Student)
      → HashPassword → store hash
    ← returns User
  → Show FloatingSuccessModal → Navigate to /login
```

### Edit User + Login with New Credentials
```
UserManagement.razor → OpenEditDialog(user)
  → Creates detached User copy with all properties
  → User edits Username and/or enters new Password
  → SaveEditUser()
    → UserService.UpdateUserWithPasswordAsync(editingUser, newPassword)
      → editingUser.PasswordHash = SHA256(newPassword)
      → Repository.UpdateAsync(editingUser)
        → Loads existing tracked entity from DB
        → Copies ALL properties explicitly (Username, PasswordHash, etc.)
        → SaveChangesAsync() → UPDATE SQL
  → User logs out → Login with new Username + new Password
    → ValidateLoginAsync(newUsername, newPassword, role)
      → SHA256(newPassword) matches stored hash → SUCCESS
```

---

## 18. Common Gotchas & Lessons Learned

### 1. Missing `@` Prefix on Razor Parameters
```razor
❌ Value="_password"           → passes literal string "_password" (shows as dots)
✅ Value="@_password"          → passes the empty string variable (shows empty)

❌ Message="_successMessage"   → shows the text "_successMessage"
✅ Message="@_successMessage"  → shows "Welcome Admin Test!"
```
**Rule**: Always use `@` when passing C# variables as component parameter values.

### 2. EF Core `SetValues()` Silently Ignores Properties
```csharp
❌ context.Entry(existing).CurrentValues.SetValues(detachedUser);
// May skip Username/PasswordHash changes silently

✅ existingUser.Username = user.Username;
   existingUser.PasswordHash = user.PasswordHash;
   // explicit = guaranteed
```
**Rule**: Use explicit property assignment for critical updates.

### 3. Blazor Server + DbContext Lifetime
```csharp
// BAD — DbContext lives as long as the circuit (stale data, tracking conflicts)
public class Repo { public Repo(AppDbContext ctx) { } }

// GOOD — fresh DbContext per operation
public class Repo {
    private readonly IServiceScopeFactory _scopeFactory;
    public async Task DoWork() {
        await using var scope = _scopeFactory.CreateAsyncScope();
        var ctx = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
}
```

### 4. Password Trimming Consistency
The password is **not trimmed** before hashing during login (`ValidateLoginAsync`), but the username IS trimmed during registration. Always ensure the same transformation is applied during both registration and login.

### 5. MudBlazor `@bind-Value` vs `Value` + `ValueChanged`
- Use `@bind-Value` for simple fields where Blazor handles the binding automatically.
- Use `Value` + `ValueChanged` for custom components (like `PasswordField`) where you need to intercept the value change.

### 6. Delete Cascading
When deleting a User with a linked Student:
1. `UserService.DeleteUserAsync` checks `user.StudentId`
2. If student exists → `_studentRepository.DeleteAsync(studentId)` first
3. Then `_repository.DeleteAsync(userId)`
4. EF Core's `DeleteBehavior.SetNull` handles the FK cleanup

---

> **End of Learnings.md** — This document covers every file, every design decision, and every logic flow in the Student Management System.
