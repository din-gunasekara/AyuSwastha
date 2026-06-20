# AyuSwastha

**Ayurveda Clinic Management System** — a Windows desktop application for traditional
Sri Lankan Ayurvedic practices. Built with **C# / Windows Forms** on **.NET Framework 4.8**,
with a local **SQLite** database.

> Coursework project for *Object Oriented Programming with C#* — emphasising OOP design,
> exception handling, and database integration.

## ★ Innovative feature — Dosha-Based Recommendation Engine
Each patient is profiled by their Prakriti (**Vata / Pitta / Kapha** and dual constitutions).
The engine suggests suitable **herbs, therapies, and diet/lifestyle guidance**, turning plain
records into practitioner decision support. See it live on the **Patients** screen — change the
*Prakriti (Dosha)* field and the guidance panel updates instantly.

## Modules
Login · Dashboard · Patient Details · Appointment Booking · Therapy Schedule ·
Prescriptions · Billing · Doctors.
_(Patient management and the dashboard are fully implemented in this scaffold; the remaining
modules are stubbed views ready to build out — see [TASK.md](TASK.md).)_

## Project structure
```
AyuSwastha.sln
src/AyuSwastha/
  Program.cs                 App entry point + global exception handling
  Core/                      Theme, password hashing, custom exceptions
  Models/                    Person (abstract) → Patient/Doctor, User, Appointment, …
  Data/                      SQLite bootstrap + seed, IRepository<T>, repositories
  Services/                  Auth, Session, DoshaRecommendationService
  Forms/                     LoginForm, MainForm + navigable UserControls
```

## Build & run
1. Open `AyuSwastha.sln` in **Visual Studio 2019/2022** on **Windows**.
2. Let NuGet restore `System.Data.SQLite.Core`.
3. Set **AyuSwastha** as the startup project and press **F5**.

The database file `AyuSwastha.db` is created automatically on first launch, with demo data.

## Default login
| Role  | Username | Password   |
|-------|----------|------------|
| Admin | `admin`  | `admin123` |

> Change the seeded credentials before any real use.
