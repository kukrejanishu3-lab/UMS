🎓 University Management System (UMS) - Backend API

An enterprise-grade, production-ready University Management System backend built with .NET 10, following Clean Architecture, CQRS (Command Query Responsibility Segregation) with MediatR, and Domain-Driven Design (DDD) principles.

🏛️ Architecture & Tech Stack
Framework: .NET 10 Web API
Architecture: Clean Architecture (Onion Architecture)
UMS.Domain - Core Entities, Enums & Business Rules
UMS.Application - CQRS Commands, Queries, MediatR Handlers & DTOs
UMS.Infrastructure - EF Core, SQL Server Migrations & DbContext
UMS.API - REST Controllers, JWT Auth & Global Error Handling
Patterns & Libraries:
CQRS with MediatR (Decoupled business logic)
Entity Framework Core 10 (Code-First Migrations)
JWT Bearer Authentication with 3-Tier Role-Based Access Control (RBAC)
Swagger / OpenAPI for Interactive API documentation
👥 3-Tier Role Hierarchy (RBAC)
SuperAdmin (Central Administration):
Full university governance (Departments, Semesters, Courses, Sections, Timetables).
Course Drop Request approval/rejection.
Semester Progression & Academic Lifecycle Engine (close-and-promote).
Teacher (Academic Faculty):
View assigned sections and teaching timetable.
Daily class attendance marking.
Section awards sheet submission (HEC marks grading).
Student (Self-Service Portal):
View enrolled courses, weekly schedule, and rooms.
Track attendance percentage and short-attendance (75%) alerts.
Drop course request submission with medical/schedule reason.
View complete multi-semester transcript with SGPA and progressive CGPA.
⚡ Core Business Engines & Features
1. 📈 Academic Lifecycle & Semester Promotion Engine
Formally closes active semesters and freezes examination results.
Calculates semester SGPA and multi-semester cumulative CGPA.
Automatically promotes eligible students to the next semester (CurrentSemester + 1).
HEC Probation Rule: Flags students with CGPA < 2.00 with an Academic Probation Warning.
Repeater Tracking: Flags failed courses (Grade F) and withdrawn courses (Grade W) for mandatory repeat.
2. 🎯 HEC Standard Grading Engine
Converts raw evaluation marks into official HEC letter grades (A, A-, B+, B, B-, C+, C, D, F) and quality points.
Single-student academic transcript view (GET /api/Student/{id}/transcript).
Section awards list retrieval for faculty.
3. 🗓️ Timetable & Scheduling Engine
Slot-by-slot class scheduling with day, timings, and room allocation.
Collision Detection: Prevents room double-booking and teacher schedule clashes.
4. 📋 Attendance Engine
Daily attendance marking with Present, Absent, and Late statuses.
Enforces the university 75% minimum attendance rule with automated detain warnings.
5. 🔄 Course Drop (Grade 'W') Workflow
Student submits formal drop request with justification.
SuperAdmin reviews and approves 
→
→ Status becomes Dropped, Grade becomes W (Withdrawn).
Excluded from current GPA calculation; flagged for future registration.