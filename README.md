# SCADA_Project
SCADA Master Project

# Getting started (no Cloud)
- Copy Project/packages/dnp3_protocol/runtime/`dnp3win32.dll` to Project/Simulator/bin/Debug
- Open up solution in `VisualStudio`
- Update database (PM> update-database) on Scada.Common and NetowkModelService projects
- Solution -> Properties -> Multiple StartUp Projects Configuration
- Set target to x86 for all projects (not AnyCPU)
- Set the following order and foreach set `Start` option
  - `ProjectWebApi`
  - `Simulator`
  - `GUI`
  - `NetworkModelServiceSelfHost`
  - `TransactionManager`
  - `ModelLabsApp`
  - `CE`
  - `NDS`
- Build and Run

# Getting started (Cloud)
## Requirements
- Visual Studio 2019
- Azure Storage Simulator
- Service Fabric

`If you have any problems related to Azure SQL, contact Vuk Isic`

## Setup
- Open up solution in `Visual Studio`
- Start Storage Emulator (Storage Emulator must be up and running)
- Start Service Fabric (Up and running & connected)
- Double click on ServiceFabric icon in Nofification area in taskbar (should open new tab in browser with Service Fabric Explorer)
- Select multiple projeects and choose `ServiceFabricApp` and `Simulator`
- Build and Run Solution
- Run `run.bat` to run GUI as separate process in Windows
- Service Fabric Explorer should look like this if everything is ok

![sfe](https://raw.githubusercontent.com/vukisic/SCADA_Project/main/docs/sfe.png)

- Visual Studio Output should look like this if everiting is ok

![sfevl](https://raw.githubusercontent.com/vukisic/SCADA_Project/main/docs/sfev.png)


# NServiceBus Licence
- Go to Solution -> Resources
- Open CMD in that folder
- `New Licence` [Download](https://drive.google.com/file/d/1nhRgNQIZ_MQHS1nUYmveLc5Y1fkz5JuY/view?usp=sharing)
- mkdir %LocalAppData%\ParticularSoftware 2> NUL
- copy /Y license.xml %LocalAppData%\ParticularSoftware

# Requirements
- VisualStudio 2019
- MSSQL Server
