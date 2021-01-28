# SCADA_Project
SCADA Master Project

# Getting started (no Cloud)
- Copy Project/packages/dnp3_protocol/runtime/`dnp3win32.dll` to Project/Simulator/bin/Debug
- Open up solution in `VisualStudio`
- Update database (PM> update-database) on Scada.Common and NetowkModelService projects
- Solution -> Properties -> Multiple StartUp Projects Configuration
- Set target to x86 for all projects (not AnyCPU)
- Set the following order and foreach set `Start` option
  - `Simulator`
  - `GUI`
  - `NetworkModelServiceSelfHost`
  - `TransactionManager`
  - `ModelLabsApp`
  - `CE`
  - `NDS`
- Build and Run

# NServiceBus Licence
- Go to Solution -> Resources
- Open CMD in that folder
- mkdir %LocalAppData%\ParticularSoftware 2> NUL
- copy /Y license.xml %LocalAppData%\ParticularSoftware

# Requirements
- VisualStudio 2019
- MSSQL Server
