library devio;

uses
  Windows,
  System.SysUtils,
  System.Classes,
  WinSvc,
  IoPackets in 'IoPackets.pas';

{$R *.res}

const
  ServiceName: PWideChar = 'fsprot';

function StartService: LongBool; stdcall; export;
var
  schService, schSCManager: Dword;
  p: PChar;
begin
  p := nil;
  schSCManager := OpenSCManager(nil, nil, SC_MANAGER_ALL_ACCESS);
  if schSCManager = 0 then
  begin
    result := false;
    exit;
  end;

  try
    schService := OpenService(schSCManager, ServiceName, SERVICE_ALL_ACCESS);

    if schService = 0 then
    begin
      result := false;
      exit;
    end;

    try
      if not WinSvc.StartService(schService, 0, p) then
      begin
        result := false;
        exit;
      end;

    finally
      CloseServiceHandle(schService);
    end;

  finally
    CloseServiceHandle(schSCManager);
  end;

  result := true;
end;

function StopService: LongBool; stdcall; export;
var
  schService, schSCManager: Dword;
  ss: _SERVICE_STATUS;
begin
  schSCManager := OpenSCManager(nil, nil, SC_MANAGER_ALL_ACCESS);

  if schSCManager = 0 then
  begin
    result := false;
    exit;
  end;

  try
    schService := OpenServiceW(schSCManager, ServiceName, SERVICE_ALL_ACCESS);

    if schService = 0 then
    begin
      result := false;
      exit;
    end;

    try
      if not ControlService(schService, SERVICE_CONTROL_STOP, ss) then
      begin
        result := false;
        exit;
      end;

    finally
      CloseServiceHandle(schService);
    end;

  finally
    CloseServiceHandle(schSCManager);
  end;

  result := true;
end;

function OpenDevice: THandle;
begin
  result := CreateFileW('\\.\disklocker_fsprotdrv', GENERIC_WRITE or
    GENERIC_READ, FILE_SHARE_READ or FILE_SHARE_WRITE, nil, OPEN_EXISTING,
    FILE_ATTRIBUTE_NORMAL, 0);
end;

procedure CloseDevice(hDevice: THandle);
begin
  CloseHandle(hDevice);
end;

function AddPathForProtection(FullPath: PWideChar; WeakProtection: LongBool; var UniqueKey: DWORD): LongBool; stdcall; export;
var
  hDevice: THandle;
begin
  hDevice := OpenDevice();

  if (hDevice = 0) or (hDevice = INVALID_HANDLE_VALUE) then
  begin
    result := false;
    exit;
  end;

  result := IoAddPathForProtection(hDevice, FullPath, WeakProtection, UniqueKey);

  CloseDevice(hDevice);
end;

function RemovePathFromProtection(UniqueKey: DWORD; var RemoveResult: LongBool): LongBool; stdcall; export;
var
  hDevice: THandle;
begin
  hDevice := OpenDevice();

  if (hDevice = 0) or (hDevice = INVALID_HANDLE_VALUE) then
  begin
    result := false;
    exit;
  end;

  result := IoRemovePathFromProtection(hDevice, UniqueKey, RemoveResult);

  CloseDevice(hDevice);
end;

function RemoveAllPathsFromProtection(): LongBool; stdcall; export;
var
  hDevice: THandle;
begin
  hDevice := OpenDevice();

  if (hDevice = 0) or (hDevice = INVALID_HANDLE_VALUE) then
  begin
    result := false;
    exit;
  end;

  result := IoRemoveAllPathsFromProtection(hDevice);

  CloseDevice(hDevice);
end;

function SetUnloadAccess(CanUnload: LongBool): LongBool; stdcall; export;
var
  hDevice: THandle;
begin
  hDevice := OpenDevice();

  if (hDevice = 0) or (hDevice = INVALID_HANDLE_VALUE) then
  begin
    result := false;
    exit;
  end;

  result := IoSetUnloadAccess(hDevice, CanUnload);

  CloseDevice(hDevice);
end;

exports
StartService,
StopService,
AddPathForProtection,
RemovePathFromProtection,
RemoveAllPathsFromProtection,
SetUnloadAccess;

end.
