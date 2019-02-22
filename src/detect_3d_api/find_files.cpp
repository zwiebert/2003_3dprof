#include "find_files.h"
#include <stdio.h>
#define WIN32_LEAN_AND_MEAN // Exclude rarely-used stuff from Windows headers
#define _WIN32_WINNT 0x0400
#include <windows.h>



const char *
find_files(const char *dir)
{
  static WIN32_FIND_DATA FindFileData;
  static HANDLE hFind;
  static bool find_in_progress = false;

  if (dir) {
    hFind = FindFirstFileEx(dir, FindExInfoStandard, &FindFileData, FindExSearchNameMatch, NULL, 0);

    if (hFind == INVALID_HANDLE_VALUE) {
      return 0;
    } else {
      find_in_progress = true;
      return FindFileData.cFileName;
    }

  } else if (find_in_progress) {
    if (FindNextFile(hFind, &FindFileData)) {
      return FindFileData.cFileName;
    } else {
      FindClose(hFind);
      find_in_progress = false;
      return 0;
    }
  }

  return 0;
}


const char *
find_dirs(const char *dir)
{
  static WIN32_FIND_DATA FindFileData;
  static HANDLE hFind;
  static bool find_in_progress = false;

  if (dir) {
    hFind = FindFirstFileEx(dir, FindExInfoStandard, &FindFileData, FindExSearchNameMatch, NULL, 0);

    if (hFind == INVALID_HANDLE_VALUE) {
      return 0;
    } else {

      while (!(FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
        if (!FindNextFile(hFind, &FindFileData)) {
          FindClose(hFind);
          find_in_progress = false;
          return 0;
        }
      }
      find_in_progress = true;
      return FindFileData.cFileName;
    }

  } else if (find_in_progress) {
    if (FindNextFile(hFind, &FindFileData)) {
      while (!(FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)) {
        if (!FindNextFile(hFind, &FindFileData))
          return 0;
      }
      return FindFileData.cFileName;
    } else {
      FindClose(hFind);
      find_in_progress = false;
      return 0;
    }
  }

  return 0;
}


struct find_file_data {
  WIN32_FIND_DATA FindFileData;
  HANDLE hFind;
  bool find_in_progress;
  bool return_previous;
};

find_files::find_files() {
  find_file_data *ffd = new find_file_data();
  ffd->find_in_progress = false;
  ffd->return_previous = false;
  this->data = ffd;
}

find_files::find_files(const char *dir) {
  find_file_data *ffd = new find_file_data();
  ffd->find_in_progress = false;
  ffd->return_previous = true;
  this->data = ffd;
  find(dir);
}

const char *
find_files::find(const char *dir) {
  find_file_data *ffd = (find_file_data *)data;

    if (ffd->find_in_progress)
      FindClose(ffd->hFind);

   ffd->hFind = FindFirstFileEx(dir, FindExInfoStandard, &ffd->FindFileData, FindExSearchNameMatch, NULL, 0);

    if (ffd->hFind == INVALID_HANDLE_VALUE) {
      return 0;
    } else {
      ffd->find_in_progress = true;
      return ffd->FindFileData.cFileName;
    }
}

const char *
find_files::find() {
  find_file_data *ffd = (find_file_data *)data;

  if (ffd->find_in_progress) {
    if (FindNextFile(ffd->hFind, &ffd->FindFileData)) {
      return ffd->FindFileData.cFileName;
    } else {
      FindClose(ffd->hFind);
      ffd->find_in_progress = false;
      return 0;
    }
  }
  return 0;
}

const char *
find_files::get_name()
{
   find_file_data *ffd = (find_file_data *)data;

   if (!ffd->find_in_progress)
     return 0;

   return ffd->FindFileData.cFileName;
}




find_dirs::find_dirs() {
  find_file_data *ffd = new find_file_data();
  ffd->find_in_progress = false;
  ffd->return_previous = false;
  this->data = ffd;
}

find_dirs::find_dirs(const char *dir) {
  find_file_data *ffd = new find_file_data();
  ffd->find_in_progress = false;
  ffd->return_previous = true;
  this->data = ffd;
  find(dir);
}

const char *
find_dirs::find(const char *dir) {
  if (!dir)
    return find();

  find_file_data *ffd = (find_file_data *)data;

  if (ffd->find_in_progress)
    FindClose(ffd->hFind);

  ffd->hFind = FindFirstFileEx(dir, FindExInfoStandard, &ffd->FindFileData, FindExSearchNameMatch, NULL, 0);

  if (ffd->hFind == INVALID_HANDLE_VALUE)
    return 0;

  ffd->find_in_progress = true;

  if (!(ffd->FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
    if (ffd->return_previous) {
      ffd->return_previous = false;
      const char *s = find();
      ffd->return_previous = true;
    } else 
      return find();

  return ffd->FindFileData.cFileName;
}


const char *
find_dirs::find() {
  find_file_data *ffd = (find_file_data *)data;

  if (ffd->return_previous && ffd->find_in_progress) {
      ffd->return_previous = false;
      return ffd->FindFileData.cFileName;
  }

  if (ffd->find_in_progress) {

    while (FindNextFile(ffd->hFind, &ffd->FindFileData)) {
      if (ffd->FindFileData.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        return ffd->FindFileData.cFileName;
    }

    FindClose(ffd->hFind);
    ffd->find_in_progress = false;
    return 0;

  }
  return 0;
}

const char *
find_dirs::get_name()
{
  find_file_data *ffd = (find_file_data *)data;

  if (!ffd->find_in_progress)
     return 0;

  return ffd->FindFileData.cFileName;
}

