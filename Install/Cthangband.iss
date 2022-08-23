[Setup]
AppCopyright=© 2022 Gurbintroll Games
AppName=Cthangband
AppPublisher=Gurbintroll Games
AppPublisherURL=https://gurbintrollgames.wordpress.com/
AppVersion=6.10
DefaultDirName={commonpf}\Cthangband
OutputBaseFilename=cthangband.6.10.install
SetupIconFile="Cthangband.ico"
ArchitecturesInstallIn64BitMode=x64
UninstallDisplayIcon={app}\Cthangband.exe
Uninstallable=true
DefaultGroupName=Cthangband
[Files]
Source: "Payload\*"; DestDir: "{app}"
Source: "Payload\Backgrounds\*"; DestDir: "{app}\Backgrounds"
Source: "Payload\Manual\*"; DestDir: "{app}\Manual"
Source: "Payload\Music\*"; DestDir: "{app}\Music"
Source: "Payload\Sounds\*"; DestDir: "{app}\Sounds"
[Icons]
Name: {group}\Cthangband; Filename: {app}\Cthangband.exe; IconFilename: {app}\Cthangband.exe
Name: {group}\Uninstall Cthangband; Filename: {app}\unins000.exe; IconFilename: {app}\unins000.exe