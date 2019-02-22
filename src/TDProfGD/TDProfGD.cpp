/*
2004-10-30/bw: Allow more than one program in a single directory. Ini-file was hardcoded to "TDProfGD.ini". It is now generated from exe name (Path\Name.exe => Path\Name.ini). 
*/

#define WIN32_LEAN_AND_MEAN		// Exclude rarely-used stuff from Windows headers
// Windows Header Files:
#include <windows.h>
// C RunTime Header Files
#include <stdlib.h>
#include <malloc.h>
#include <memory.h>
#include <tchar.h>
#include <cstring>
#include <cstdio>

using namespace std;


static char tdprof_path[512];
static char prof_name[512];
static char buf[1024];

BOOL get_ini_file_path(char *buf, size_t size)
{
  DWORD res = GetModuleFileName(0, buf, (DWORD)size);
  if (res == 0 || res == size)
    return FALSE;
  strcpy(&buf[strlen(buf) - 3], "ini");
  return TRUE;
}


void test_create_process(char *cmdline, const char *wdir)
{
  BOOL success;
  STARTUPINFO si = {sizeof (STARTUPINFO), };
  PROCESS_INFORMATION procInfo = {0,};

  success = CreateProcess(0, cmdline, 0, 0, FALSE, 0, 0, wdir, &si, &procInfo);

  if (success)
    SetPriorityClass(procInfo.hProcess, GetPriorityClass(GetCurrentProcess()));

  WaitForSingleObject(procInfo.hProcess, INFINITE);

  CloseHandle(procInfo.hProcess);
  CloseHandle(procInfo.hThread);

}


#define TAG_TDPROF_DIR "tdprof_dir"
#define TAG_PROF_NAME "prof_name"

bool
parse_ini_file(const char *name)
{
  bool found_tdprof_dir = false;
  bool found_prof_name = false;
  FILE *is;
  if ((is = fopen(name, "r"))) {
    char *line;
    while ((line = fgets(buf, sizeof buf, is))) {
      line[strlen(line)-1] = '\0';
      if (0 == strncmp(TAG_TDPROF_DIR "=", line, strlen (TAG_TDPROF_DIR "="))) {
        strcpy(tdprof_path, line + strlen (TAG_TDPROF_DIR "=")); 
        found_tdprof_dir = true;
      } else if (0 == strncmp(TAG_PROF_NAME "=", line, strlen (TAG_PROF_NAME "="))) {
        strcpy(prof_name, line + strlen (TAG_PROF_NAME "="));
        found_prof_name = true;
      }
    }
    fclose(is);
  }
  return found_prof_name && found_tdprof_dir;
}



int APIENTRY _tWinMain(HINSTANCE hInstance,
                       HINSTANCE hPrevInstance,
                       LPTSTR    lpCmdLine,
                       int       nCmdShow)
{
  int result = 1;

  if (get_ini_file_path(buf, sizeof buf) && parse_ini_file(buf)) {
    sprintf(buf, "%s/tdprof.exe -run %s -- %s", tdprof_path, prof_name, lpCmdLine);
    test_create_process(buf, tdprof_path);
    result = 0; // success
  }


  return result;
}




