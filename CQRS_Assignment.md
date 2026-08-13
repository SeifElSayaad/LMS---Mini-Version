# CQRS Practice Assignment — MediatR Queries & Commands Migration

Welcome, Intern! Your mission is to migrate **4 Read operations (Queries)** and **4 Write operations (Commands)** to MediatR Query/Command/Handler pairs.

> ⚠️ **Important Requirement for Commands:** All 4 Commands must return **nothing** (implement `IRequest`).

---

## 🔗 Project Details & Base Branch

* **GitHub Repository:** [https://github.com/Amr-shawky/LMS---Mini-Version](https://github.com/Amr-shawky/LMS---Mini-Version)
* **Starting Branch:** `mediator-scaling-problem`

> **Getting Started:** Clone or fork the repository and checkout the `mediator-scaling-problem` branch before implementing your tasks:
> ```bash
> git checkout mediator-scaling-problem
> ```

---

## Architecture Overview

You need to build all 3 pieces for each task:

```
Controller → Request (Query/Command) + IMediator.Send(...) → Handler → Repository / Service → Database
   ↑                          ↑                                ↑
 STEP 3                     STEP 1                           STEP 2
```

- **Queries** return data (DTOs).
- **Commands** perform state changes and return **nothing** (`IRequest`).

---

## Key Files to Reference

| File | Purpose |
|---|---|
| `Domain/Entities/` | Entity classes (`Track`, `Intern`, `Enrollment`, `Payment`) |
| `Domain/Repositories/IGeneralRepository.cs` | Repository interface — `GetTable()`, `GetByIdAsync()`, `AddAsync()`, `UpdateAsync()`, `DeleteAsync()` |
| `Services/Interfaces/` | Business domain services (`ITrackService`, `IInternService`, `IEnrollmentService`, `IPaymentService`) |
| `DTOs/` | DTO classes (`TrackDto`, `InternDto`, `EnrollmentDto`, `PaymentDto`) |
| `Mapping/MappingExtensions.cs` | Mapping methods (`.ToDto()`, `.ToSummaryViewModel()`, etc.) |

---

# Part 1: Query Tasks (Read Operations — Return DTOs)

### Task 1: `GetTrackByIdQuery` → Get a Single Track by ID

| | |
|---|---|
| **Query File** | Create: `Features/Tracks/Queries/GetTrackByIdQuery.cs` |
| **Handler File** | Create: `Features/Tracks/Handlers/GetTrackByIdQueryHandler.cs` |
| **Controller** | Wire: `TrackController.GetById(int id)` |
| **Business Goal** | Admin needs to view the full details of a specific training track. |
| **Query Input** | `int Id` |
| **Handler Returns** | `TrackDto?` |
| **Swagger Endpoint** | `GET /api/Track/{id}` |

**What to do:**
1. Create the Query: `public record GetTrackByIdQuery(int Id) : IRequest<TrackDto?>;`
2. Create the Handler: implement `IRequestHandler<GetTrackByIdQuery, TrackDto?>`
3. Inside `Handle()`: fetch track using `_trackService.GetByIdAsync(request.Id)` or repository and return mapped `TrackDto`
4. In Controller: `var result = await _mediator.Send(new GetTrackByIdQuery(id));` return `Ok(...)` or `NotFound()`

---

### Task 2: `GetAllInternsQuery` → List All Interns

| | |
|---|---|
| **Query File** | Create: `Features/Interns/Queries/GetAllInternsQuery.cs` |
| **Handler File** | Create: `Features/Interns/Handlers/GetAllInternsQueryHandler.cs` |
| **Controller** | Wire: `InternController.GetAll()` |
| **Business Goal** | Admin needs to see a summary list of all registered interns with their assigned track names. |
| **Query Input** | None (parameterless) |
| **Handler Returns** | `IEnumerable<InternDto>` |
| **Swagger Endpoint** | `GET /api/Intern` |

**What to do:**
1. Create the Query: `public record GetAllInternsQuery : IRequest<IEnumerable<InternDto>>;`
2. Create the Handler: implement `IRequestHandler<GetAllInternsQuery, IEnumerable<InternDto>>`
3. Inside `Handle()`: use `_internRepository.GetTable().Include(i => i.Track)` to fetch all interns and map to `InternDto`
4. In Controller: `var result = await _mediator.Send(new GetAllInternsQuery());` return `Ok(...)`

---

### Task 3: `GetInternByIdQuery` → Get a Single Intern by ID

| | |
|---|---|
| **Query File** | Create: `Features/Interns/Queries/GetInternByIdQuery.cs` |
| **Handler File** | Create: `Features/Interns/Handlers/GetInternByIdQueryHandler.cs` |
| **Controller** | Wire: `InternController.GetById(int id)` |
| **Business Goal** | Admin needs to view full details of a specific intern (name, email, birth year, track info). |
| **Query Input** | `int Id` |
| **Handler Returns** | `InternDto?` (nullable — null if not found) |
| **Swagger Endpoint** | `GET /api/Intern/{id}` |

**What to do:**
1. Create the Query: `public record GetInternByIdQuery(int Id) : IRequest<InternDto?>;`
2. Create the Handler: implement `IRequestHandler<GetInternByIdQuery, InternDto?>`
3. Inside `Handle()`: fetch intern by Id including track details, map to `InternDto`
4. In Controller: `var result = await _mediator.Send(new GetInternByIdQuery(id));` return `Ok(...)` or `NotFound()`

---

### Task 4: `GetEnrollmentsByInternQuery` → Get All Enrollments for a Specific Intern

| | |
|---|---|
| **Query File** | Create: `Features/Enrollments/Queries/GetEnrollmentsByInternQuery.cs` |
| **Handler File** | Create: `Features/Enrollments/Handlers/GetEnrollmentsByInternQueryHandler.cs` |
| **Controller** | Wire: `EnrollmentController.GetByIntern(int internId)` |
| **Business Goal** | Admin needs to view the enrollment history of a particular intern. |
| **Query Input** | `int InternId` |
| **Handler Returns** | `IEnumerable<EnrollmentDto>` |
| **Swagger Endpoint** | `GET /api/Enrollment/intern/{internId}` |

**What to do:**
1. Create the Query: `public record GetEnrollmentsByInternQuery(int InternId) : IRequest<IEnumerable<EnrollmentDto>>;`
2. Create the Handler: implement `IRequestHandler<GetEnrollmentsByInternQuery, IEnumerable<EnrollmentDto>>`
3. Inside `Handle()`: fetch enrollments filtering by `InternId`, map to `EnrollmentDto` list
4. In Controller: `var result = await _mediator.Send(new GetEnrollmentsByInternQuery(internId));` return `Ok(...)`

---

# Part 2: Command Tasks (Write Operations — Return Nothing / `IRequest`)

### Task 5: `UpdateTrackCommand` → Update Track Details

| | |
|---|---|
| **Command File** | Create: `Features/Tracks/Commands/UpdateTrackCommand.cs` |
| **Handler File** | Create: `Features/Tracks/Handlers/UpdateTrackCommandHandler.cs` |
| **Controller** | Wire: `TrackController.Update(int id, UpdateTrackViewModel vm)` |
| **Business Goal** | Admin needs to update track details (name, fees, active status, capacity). |
| **Command Input** | `int Id, string Name, decimal Fees, bool IsActive, int MaxCapacity` |
| **Handler Returns** | **Nothing** (`IRequest`) |
| **Swagger Endpoint** | `PUT /api/Track/{id}` |

**What to do:**
1. Create Command: `public record UpdateTrackCommand(int Id, string Name, decimal Fees, bool IsActive, int MaxCapacity) : IRequest;`
2. Create Handler: implement `IRequestHandler<UpdateTrackCommand>`
3. Inside `Handle()`: update track details in database via service or repository and commit unit of work
4. In Controller: `await _mediator.Send(new UpdateTrackCommand(id, vm.Name, vm.Fees, vm.IsActive, vm.MaxCapacity));` return `NoContent()`

---

### Task 6: `DeleteTrackCommand` → Delete a Track

| | |
|---|---|
| **Command File** | Create: `Features/Tracks/Commands/DeleteTrackCommand.cs` |
| **Handler File** | Create: `Features/Tracks/Handlers/DeleteTrackCommandHandler.cs` |
| **Controller** | Wire: `TrackController.Delete(int id)` |
| **Business Goal** | Admin needs to remove a track from the system. |
| **Command Input** | `int Id` |
| **Handler Returns** | **Nothing** (`IRequest`) |
| **Swagger Endpoint** | `DELETE /api/Track/{id}` |

**What to do:**
1. Create Command: `public record DeleteTrackCommand(int Id) : IRequest;`
2. Create Handler: implement `IRequestHandler<DeleteTrackCommand>`
3. Inside `Handle()`: remove track by Id from repository and save changes
4. In Controller: `await _mediator.Send(new DeleteTrackCommand(id));` return `NoContent()`

---

### Task 7: `CancelEnrollmentCommand` → Cancel Enrollment & Refund Payment

| | |
|---|---|
| **Command File** | Create: `Features/Enrollments/Commands/CancelEnrollmentCommand.cs` |
| **Handler File** | Create: `Features/Enrollments/Handlers/CancelEnrollmentCommandHandler.cs` |
| **Controller** | Wire: `EnrollmentController.Cancel(int id)` |
| **Business Goal** | Admin needs to cancel an intern's enrollment and process payment refund. |
| **Command Input** | `int EnrollmentId` |
| **Handler Returns** | **Nothing** (`IRequest`) |
| **Swagger Endpoint** | `POST /api/Enrollment/{id}/cancel` |

**What to do:**
1. Create Command: `public record CancelEnrollmentCommand(int EnrollmentId) : IRequest;`
2. Create Handler: implement `IRequestHandler<CancelEnrollmentCommand>`
3. Inside `Handle()`: call `_enrollmentService.UpdateStatusAsync` to mark cancelled, `_paymentService.RefundPaymentAsync`, then commit via `_unitOfWork.CompleteAsync()`
4. In Controller: `await _mediator.Send(new CancelEnrollmentCommand(id));` return `Ok()` or `NoContent()`

---

### Task 8: `TransferEnrollmentCommand` → Transfer Enrollment to a New Track

| | |
|---|---|
| **Command File** | Create: `Features/Enrollments/Commands/TransferEnrollmentCommand.cs` |
| **Handler File** | Create: `Features/Enrollments/Handlers/TransferEnrollmentCommandHandler.cs` |
| **Controller** | Wire: `EnrollmentController.Transfer(int id, int newTrackId)` |
| **Business Goal** | Move an intern's enrollment to a different track and adjust payment fees. |
| **Command Input** | `int EnrollmentId`, `int NewTrackId` |
| **Handler Returns** | **Nothing** (`IRequest`) |
| **Swagger Endpoint** | `POST /api/Enrollment/{id}/transfer/{newTrackId}` |

**What to do:**
1. Create Command: `public record TransferEnrollmentCommand(int EnrollmentId, int NewTrackId) : IRequest;`
2. Create Handler: implement `IRequestHandler<TransferEnrollmentCommand>`
3. Inside `Handle()`: check new track capacity, update enrollment track, adjust payment amount, and commit unit of work
4. In Controller: `await _mediator.Send(new TransferEnrollmentCommand(id, newTrackId));` return `Ok()` or `NoContent()`

---

## Verification Steps

1. Run the application: `dotnet run` from the project root
2. Open Swagger: Navigate to `https://localhost:{port}/swagger`
3. Test each endpoint:
   - Verify Queries return `200 OK` with JSON data
   - Verify Commands perform the required database modifications and return `204 NoContent` or `200 OK`

## Rules

- DO NOT use `ConfigureAwait(false)` — it's default in modern .NET
- Handlers for Queries return DTOs (e.g., `TrackDto`, `InternDto`) — NOT ViewModels
- Handlers for Commands implement `IRequestHandler<TCommand>` and return **void / nothing**
- Use async/await for all DB calls
- Pass `cancellationToken` to all async EF Core methods

---

## 📩 How to Submit Your Assignment

Once you have completed and verified all tasks, submit your solution via email to Mentor Amr.

* **Recipient Email:** `amrshawky936@gmail.com`
* **Subject Format:** `[Assignment #<Number>] - <Full Name> - <Group/Track>`
  * *Example:* `[Assignment #3] - Ahmed Hassan - .NET Saturday (8-11 PM)`
* **Base Repository:** `https://github.com/Amr-shawky/LMS---Mini-Version`
* **Base Branch:** `mediator-scaling-problem`
* **Code Submission Requirements:**
  - If you **forked** the repository: attach your **Forked GitHub Repository Link**.
  - If working in the **same repository**: specify your **Branch Name** created off `mediator-scaling-problem`.

### Email Submission Template

Copy and complete the following template when sending your email:

```text
Subject: [Assignment #3] - [Your Full Name] - [Group / Track Name]

Dear Mentor Amr,

Please find my submission for CQRS Practice Assignment (MediatR Query/Command Migration).

Student Information:
- Full Name: [Your Full Name]
- Group / Track: [e.g., .NET Saturday 8-11 PM]
- Submission Method: [Forked Repo / Same Repo Branch]
- GitHub Repo Link: [https://github.com/your-username/your-repo-name] (Required if forked)
- Branch Name: [e.g., feature/ahmed-hassan-cqrs] (Required if working in same repo)
- Pull Request Link (Optional): [Link to PR if applicable]

Completed Tasks Checklist:
[X] Task 1: GetTrackByIdQuery (GET /api/Track/{id})
[X] Task 2: GetAllInternsQuery (GET /api/Intern)
[X] Task 3: GetInternByIdQuery (GET /api/Intern/{id})
[X] Task 4: GetEnrollmentsByInternQuery (GET /api/Enrollment/intern/{internId})
[X] Task 5: UpdateTrackCommand (PUT /api/Track/{id})
[X] Task 6: DeleteTrackCommand (DELETE /api/Track/{id})
[X] Task 7: CancelEnrollmentCommand (POST /api/Enrollment/{id}/cancel)
[X] Task 8: TransferEnrollmentCommand (POST /api/Enrollment/{id}/transfer/{newTrackId})

Notes / Comments (Optional):
- [Mention any challenges encountered or additional improvements made]

Best regards,
[Your Full Name]
```
