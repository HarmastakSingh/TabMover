; Inno Setup installer definition.
#define AppName "TabMover"
#define AppVersion "0.3.0-alpha"
#define AppExeName "TabMover.exe"

[Setup]
AppId={{C65DAA8E-8E0B-4D12-98D0-3C80F49A1303}
AppName={#AppName}
AppVersion={#AppVersion}
DefaultDirName={autopf}\TabMover
DefaultGroupName=TabMover
OutputDir=..\dist
OutputBaseFilename=TabMover-Setup-v0.3.0-alpha
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
PrivilegesRequired=lowest
WizardStyle=modern
Compression=lzma2
SolidCompression=yes

[Tasks]
Name: "desktopicon"; Description: "Create a desktop shortcut"; GroupDescription: "Additional shortcuts:"
Name: "startup"; Description: "Start TabMover when I sign in to Windows"; GroupDescription: "Startup:"

[Files]
Source: "..\publish\TabMover.exe"; DestDir: "{app}"; Flags: ignoreversion
Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\TabMover"; Filename: "{app}\{#AppExeName}"
Name: "{autodesktop}\TabMover"; Filename: "{app}\{#AppExeName}"; Tasks: desktopicon

[Registry]
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "TabMover"; ValueData: "{app}\TabMover.exe"; Tasks: startup; Flags: uninsdeletevalue

[Run]
Filename: "{app}\{#AppExeName}"; Description: "Launch TabMover"; Flags: postinstall nowait skipifsilent
