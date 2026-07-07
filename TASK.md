# AyuSwastha — Ayurveda Clinic Management System · Task Board

> **Course:** Object Oriented Programming with C#
> **Domain:** Traditional Sri Lankan Ayurvedic practice management
> **Stack:** C# · Windows Forms · .NET Framework 4.8 · SQLite (via `System.Data.SQLite`)

The system manages patient profiles, herbal prescriptions, therapy schedules,
appointments, billing, and doctors — with a strong emphasis on OOP design,
exception handling, and database integration.

---

## ⭐ Innovative Feature — Dosha-Based Recommendation Engine

Conventional clinic systems are generic record-keepers. AyuSwastha's differentiator
is a **Prakriti (Dosha) analysis engine**: each patient is profiled as
**Vata / Pitta / Kapha** (or a dual constitution), and the system automatically
suggests suitable **herbs, therapies, and dietary/lifestyle guidance** aligned with
traditional Ayurvedic principles. This turns raw records into decision support for
the practitioner.

- [ ] Short Prakriti questionnaire → computes dominant Dosha
- [ ] Recommendation service maps Dosha → herbs / therapies / diet
- [ ] Recommendations surface on the Patient Details + Prescription screens
- [ ] Practitioner can accept a suggestion straight into a prescription

---

## Milestones

### M1 — Foundation
- [ ] Solution + WinForms project skeleton (`AyuSwastha.sln`)
- [ ] Shared green theme / colors (`Core/Theme.cs`)
- [ ] SQLite database bootstrap + schema + seed data (`Data/Database.cs`)
- [ ] Base OOP model layer (`Person` → `Patient` / `Doctor`, enums)

### M2 — Core Modules
- [ ] **Login** — role-based auth (Admin / Doctor / Receptionist), hashed passwords
- [ ] **Dashboard** — KPI tiles (today's appointments, patients, revenue) + nav
- [ ] **Patient Details** — CRUD, tabs (Personal, Medical History, Allergies, Lifestyle, Notes), photo upload
- [ ] **Doctors** — CRUD, specialization, availability
- [ ] **Appointment Booking** — pick patient + doctor + slot, status tracking
- [ ] **Therapy Schedule** — therapy catalog (Abhyanga, Shirodhara, …), session calendar
- [ ] **Prescriptions** — herbal items (herb, dosage, duration, instructions) + Dosha suggestions
- [ ] **Billing** — line items (consultation, therapy, herbs), totals, payment status, printable invoice

### M3 — Quality & Polish
- [ ] Centralized exception handling + user-friendly error dialogs
- [ ] Input validation on all forms (required fields, phone, dates, ranges)
- [ ] Search / filter on patient, appointment, and billing lists
- [ ] Reports/export (patient summary, daily revenue) — CSV/print
- [ ] Consistent UX, keyboard navigation, empty/loading states

### M4 — Deliverables
- [ ] SRS: functional requirements + analysis writeup
- [ ] Design: class diagram, ER diagram, use-case + sequence diagrams
- [ ] Demo data + walkthrough script for presentation
- [ ] README with build/run instructions

---

## OOP Concepts Checklist (must be demonstrated)
- [ ] **Encapsulation** — private fields, validated properties
- [ ] **Inheritance** — `Person` base → `Patient`, `Doctor`
- [ ] **Polymorphism** — overridden `ToString()` / display logic, virtual members
- [ ] **Abstraction** — `abstract Person`, interfaces (`IRepository<T>`)
- [ ] **Interfaces & generics** — generic repository / service contracts
- [ ] **Composition** — `Prescription` owns `PrescriptionItem`s; `Bill` owns `BillItem`s
- [ ] **Enums** — `Gender`, `DoshaType`, `AppointmentStatus`, `UserRole`, `PaymentStatus`
- [ ] **Exception handling** — custom exceptions, try/catch around DB + IO

## Database Integration Checklist
- [ ] Schema created programmatically on first run
- [ ] Parameterized queries (no SQL injection)
- [ ] Repository pattern separating data access from UI
- [ ] Referential integrity (foreign keys: appointment → patient/doctor, etc.)
- [ ] Seed script for demo data

---

## Suggested Data Model (entities)
`User` · `Person` (abstract) · `Patient` · `Doctor` · `Appointment` ·
`Therapy` · `TherapySession` · `Prescription` + `PrescriptionItem` ·
`Bill` + `BillItem`

## Build & Run
1. Open `AyuSwastha.sln` in **Visual Studio 2019/2022** on **Windows**.
2. Restore NuGet packages (`System.Data.SQLite.Core` restores automatically).
3. Set **AyuSwastha** as the startup project and press **F5**.

Or from a Developer Command Prompt:
```bat
nuget restore AyuSwastha.sln
msbuild AyuSwastha.sln /p:Configuration=Debug
src\AyuSwastha\bin\Debug\AyuSwastha.exe
```
> Targets **.NET Framework 4.8** (ships with Windows 10/11).
> First launch creates `AyuSwastha.db` and seeds a default admin — see README.

## Default Login (seed)
| Role  | Username | Password  |
|-------|----------|-----------|
| Admin | `admin`  | `admin123` |

_Change seeded credentials before any real use._
