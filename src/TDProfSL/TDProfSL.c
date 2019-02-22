#define WIN32_LEAN_AND_MEAN // Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#define WINVER 0x0501
#include <windows.h>
// C RunTime Header Files
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>
#include <string.h>
#include <time.h>


// global vars
BOOL win_nt;
BOOL autorestore_force_dialog;
BOOL autorestore_disable_dialog;


/// Do Restore by executing TDProf.exe -restore_now
void start_tdprof_for_restore(const char *restore_command)
{

  char *exe_path = "./tdprof.exe";
  char *exe_args = "./tdprof.exe -restore_now";
  char *wdir     = ".";
  BOOL success;



  STARTUPINFO si = {sizeof (STARTUPINFO), };
  PROCESS_INFORMATION procInfo = {0,};

  if (restore_command)
    success = CreateProcess(NULL, (LPTSTR)restore_command, 0, 0, FALSE, 0, 0, wdir, &si,  &procInfo);
  else
    success = CreateProcess(exe_path, exe_args, 0, 0, FALSE, 0, 0, wdir, &si,  &procInfo);

  CloseHandle(procInfo.hProcess);
  CloseHandle(procInfo.hThread);

}

enum AskUser { AU_LAUNCHER_DETECTED, AU_FORCE_DIALOG };

void ask_user(enum AskUser e)
{
  switch (e) {
  case AU_LAUNCHER_DETECTED:
    MessageBox (NULL, "The Game-Exe exited very soon. Maybe it was started by a launcher.\r\n"
      "Please press OK, after the Game finished. After that, 3D Settings will be restored.\r\n"
      "\r\rHint: To disable Auto Restore...\r\n"
      "...for this game: Add the option -no_restore as argument to TDProf.exe in the Games Shell Link\r\n"
      "...globally: Uncheck the related option in menu Options => Settings on the Shell Links tab\r\n", 
      "TDProfSL - Auto Restore", MB_OK);
    break;
  case AU_FORCE_DIALOG:
    MessageBox (NULL, "You have configured 3DProf to force this dialog.\r\n"
      "After the Game has finished, press OK. After that, options will be restored\r\n"
      "\r\rHint: Configure this in menu Profile | Auto Restore | Force Dialog Box\r\n",
      "TDProfSL - Auto Restore", MB_OK);
    break;

  }
}

// Start game
void test_create_process(char *a_cmdLine)
{

  char sep[] = { a_cmdLine[0], 0 };
  char *exe_path = strtok(a_cmdLine, sep);
  char *wdir     = strtok(NULL, sep);
  char *restore_command  = strtok(NULL, sep);
  char *flags = strtok(NULL, sep);
  BOOL success;
  STARTUPINFO si = {sizeof (STARTUPINFO), };
  PROCESS_INFORMATION procInfo = {0,};
  time_t s_time, e_time, duration;

  if (flags) {
    int f = atoi(flags);
    autorestore_force_dialog = (f & (1 << 0)) != 0;
    autorestore_disable_dialog = (f & (1 << 1)) != 0;
  }

  time(&s_time);

  success = CreateProcess(0, exe_path, 0,       0,   
    FALSE, 0, 0, wdir, &si,  &procInfo);

  if (win_nt)
    SetProcessWorkingSetSize(GetCurrentProcess(), -1, -1);

  // inherit our process priority class to child process
  if (success)
    SetPriorityClass(procInfo.hProcess, GetPriorityClass(GetCurrentProcess()));

  if (!autorestore_force_dialog)
    WaitForSingleObject(procInfo.hProcess, INFINITE);
  time(&e_time);

  duration = e_time - s_time;

  CloseHandle(procInfo.hProcess);
  CloseHandle(procInfo.hThread);

  if (!autorestore_disable_dialog && (duration < 120 || autorestore_force_dialog)) {
    ask_user(autorestore_force_dialog ? AU_FORCE_DIALOG : AU_LAUNCHER_DETECTED);
  }
  start_tdprof_for_restore(restore_command);

}



// Application Entry Point
int APIENTRY _tWinMain(HINSTANCE hInstance,
                       HINSTANCE hPrevInstance,
                       LPTSTR    lpCmdLine,
                       int       nCmdShow)
{
  OSVERSIONINFO osvi =  { sizeof(OSVERSIONINFO), };

  if (lpCmdLine[0] == 0)
    return 1;

  if (GetVersionEx(&osvi)) {
    win_nt = (osvi.dwPlatformId == VER_PLATFORM_WIN32_NT);
  }

  test_create_process(lpCmdLine);
  return 0;
}
