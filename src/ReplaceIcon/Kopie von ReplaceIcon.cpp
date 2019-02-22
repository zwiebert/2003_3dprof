/*******************************************************************************
* The contents of this file are subject to the Mozilla Public License Version  *
* 1.0 (the "License"); you may not use this file except in compliance with the *
* License. You may obtain a copy of the License at http://www.mozilla.org/MPL/ *
*                                                                              *
* Software distributed under the License is distributed on an "AS IS" basis,   *
* WITHOUT WARRANTY OF ANY KIND, either express or implied. See the License for *
* the specific language governing rights and limitations under the License.    *
*                                                                              *
* The Original Code is IcoUtils.pas.                                           *
*                                                                              *
* The Initial Developer of the Original Code is Miha Remec,                    *
*   http://www.miharemec.com/                                                  *
*                                                                              *
* Last modified: 2000/06/30                                                    *
*                                                                              *
* Contributor(s):                                                              *
* 2003-10-21: Translated from Delphi to C++ by Bert Winkelmann <bertw@gmx.net> *
*                                                                              *
*                                                                              *
*                                                                              *
*                                                                              *
*******************************************************************************/

#include <windows.h>
#include "Icon.h"
class CIconFile {
private:
  int      FSize;    // .ico-file size (or FImage size, which is the same)
  PBYTE    FImage;   // holds the entire .ico-file content
  PICONDIR FIconDir;
  int      FCount;   // number of images

public:
  CIconFile(const char *FileName); // construct object from an .ico file
  ~CIconFile() { delete FImage; }; // destruct object

  int GetCount() const { return FCount; } // Get number of images
  BOOL IsValid();  // [bw]Kind of IsOK() function. Should be done by ctor IMHO[bw]
  void GetIconDirEntry(int Index, ICONDIRENTRY &Result); // Get copy of DirEntry by Index
  void IconData(int Index, PBYTE &Data, int &Size); // Get copy of IconData by Index

  convert (GRPICONDIR &grpIconDir);  
};

BOOL ReplaceIcon(const char *ExeFileName,
                 const char *IcoFileName,
                 const char *ResName);


//////////////////////////////////////////////////////////////////////////////////////
// implementation
//////////////////////////////////////////////////////////////////////////////////////

#include <cstring>
#include <cstdio>
#include <cstdlib>
using namespace std;
#include <tchar.h>



CIconFile::CIconFile(const char *FileName)
: FImage(0)
{
  bool success = false;
  FILE *in = fopen(FileName, "rb");
  if (in) { 
    if ((fseek(in, 0, SEEK_END) == 0)
      && ((FSize = ftell(in)) > 0)
      && (fseek(in, 0, SEEK_SET) == 0)
      && (fread((FImage = new BYTE[FSize]), 1, FSize, in) == FSize)) {
        success = true;
        FIconDir = PICONDIR(FImage);
        FCount = FIconDir->idCount;
      }
      fclose(in);
  }
  if (!success)
    throw new int(42);
}

BOOL
CIconFile::IsValid()
{
  PICONDIR Dir;
  ICONDIRENTRY DirEntry;
  int CalculatedSize;

  BOOL Result = (FImage != NULL);

  if (Result) {
    Dir = PICONDIR(FImage);

    // check standard values
    Result = (Dir->idReserved == 0) && (Dir->idType == 1);
    if (Result) {
      // calculate icon file size
      CalculatedSize = sizeof (ICONDIR) + (Dir->idCount - 1) * sizeof (ICONDIRENTRY);
      for (int i=0; (i < Dir->idCount); ++i) {
        GetIconDirEntry(i, DirEntry);
        CalculatedSize += DirEntry.dwBytesInRes;
      }
      // check against image size
      Result = (FSize == CalculatedSize);
    }
  }
  return Result;
}


void
CIconFile::GetIconDirEntry(int Index, ICONDIRENTRY &Result)
{
  Result = FIconDir->idEntries[Index];
}

void
CIconFile::IconData(int Index, PBYTE &Data, int &Size)
{
  ICONDIRENTRY FIconDirEntry;
  GetIconDirEntry(Index, FIconDirEntry);
  Size = FIconDirEntry.dwBytesInRes;
  Data = FImage;
  Data += FIconDirEntry.dwImageOffset;
}

BOOL
CIconFile::convert(GRPICONDIR &grpIconDir)
{

}


void LogError(const char *Msg)
{
  fprintf(stderr, "error: %s\n", Msg);
  exit(EXIT_FAILURE);
}

BOOL ReplaceIcon(const char *ExeFileName,
                 const char *IcoFileName,
                 const char *ResName)
{
  BOOL success = FALSE;
  HMODULE hExe;
  HRSRC hRes;
  HGLOBAL hResLoad;
  HANDLE hUpdate;
  PGRPICONDIR GrpIconDir;
  int ResSize;  // size of icon directory resource

  GRPICONDIRENTRY GrpIconDirEntry;
  int i;
  WORD Language;
  ICONDIRENTRY IconDirEntry;
  PBYTE IconData;
  int IconSize;

  hUpdate = BeginUpdateResource(PCHAR(ExeFileName), FALSE);
  if (hUpdate == 0)
    LogError("Updating resources is not allowed or file does not exists");

  hExe = LoadLibraryEx(PCHAR(ExeFileName), 0, LOAD_LIBRARY_AS_DATAFILE);
  if (hExe == 0)
    LogError("Could not load exe");

  Language = GetIconInfo((HICON)hExe, (PICONINFO)ResName);

  // delete old icons
  hRes = FindResource(hExe, PCHAR(ResName), RT_GROUP_ICON);
  if (hRes != 0) {
    hResLoad = LoadResource(hExe, hRes);
    if (hResLoad == 0)
      LogError("Coult not load resource");

    GrpIconDir = (PGRPICONDIR)LockResource(hResLoad);
    if (GrpIconDir == NULL)
      LogError("Could not lock resource");

    for (i=0; i < GrpIconDir->idCount; ++i) {
      GrpIconDirEntry = GrpIconDir->idEntries[i];
      if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(GrpIconDirEntry.nID), Language, NULL, 0))
        LogError("Could not update (delete old) resource");
    }
  }


  // add new icons
  CIconFile IconFile(IcoFileName);
  try {
    if (!IconFile.IsValid())
      LogError("Icon file is invalid");

    ResSize = sizeof (GRPICONDIR) + ((IconFile.GetCount() - 1) * sizeof (GRPICONDIRENTRY));
    GrpIconDir = (PGRPICONDIR) realloc(0, ResSize);
    GrpIconDir->idReserved = 0;
    GrpIconDir->idType = 1;
    GrpIconDir->idCount = IconFile.GetCount();

    for(i=0; i < IconFile.GetCount(); ++i) {
      IconFile.GetIconDirEntry(i, IconDirEntry);
      IconDirEntry.dwImageOffset = 0;
      memmove(&GrpIconDir->idEntries[i], &IconDirEntry, sizeof(GRPICONDIRENTRY));
      GrpIconDir->idEntries[i].nID = i;

      IconFile.IconData(i, IconData, IconSize);

      if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(i), Language, IconData, IconSize))
        LogError("Could not update (add new) resource");
    }

    if (!UpdateResource(hUpdate, RT_GROUP_ICON, PCHAR(ResName),
      Language, GrpIconDir, ResSize))
      LogError("Could not update resource");

    free(GrpIconDir);
    success = TRUE;
  } catch (...) { }

  // free exe before commiting updates
  if (!FreeLibrary(hExe))
    LogError("Could not free executable");

  // commit updates
  if (!EndUpdateResource(hUpdate, FALSE))
    LogError("Could not write changes to file");

  return success;
}

BOOL CopyIcon(const char *SrcFileName,
              const char *DstFileName,
              const char *ResName)
{
  BOOL success = FALSE;
  HMODULE hExe;
  HRSRC hRes;
  HGLOBAL hResLoad;
  HANDLE hUpdate;
  PGRPICONDIR GrpIconDir;
  int ResSize;  // size of icon directory resource

  GRPICONDIRENTRY GrpIconDirEntry;
  int i;
  WORD Language;
  ICONDIRENTRY IconDirEntry;
  PBYTE IconData;
  int IconSize;

  hUpdate = BeginUpdateResource(PCHAR(SrcFileName), FALSE);
  if (hUpdate == 0)
    LogError("Updating resources is not allowed or file does not exists");

  hExe = LoadLibraryEx(PCHAR(DstFileName), 0, LOAD_LIBRARY_AS_DATAFILE);
  if (hExe == 0)
    LogError("Could not load exe");

  Language = GetIconInfo((HICON)hExe, (PICONINFO)ResName);

  // delete old icons
  hRes = FindResource(hExe, PCHAR(ResName), RT_GROUP_ICON);
  if (hRes != 0) {
    hResLoad = LoadResource(hExe, hRes);
    if (hResLoad == 0)
      LogError("Coult not load resource");

    GrpIconDir = (PGRPICONDIR)LockResource(hResLoad);
    if (GrpIconDir == NULL)
      LogError("Could not lock resource");

    for (i=0; i < GrpIconDir->idCount; ++i) {
      GrpIconDirEntry = GrpIconDir->idEntries[i];
      if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(GrpIconDirEntry.nID), Language, NULL, 0))
        LogError("Could not update (delete old) resource");
    }
  }

  // free exe before commiting updates
  if (!FreeLibrary(hExe))
    LogError("Could not free executable");

  // commit updates
  if (!EndUpdateResource(hUpdate, FALSE))
    LogError("Could not write changes to file");

  return success;
}


int _tmain(int argc, _TCHAR* argv[])
{
  if (argc == 3) { 
    if (ReplaceIcon(argv[1], argv[2],  "MAINICON"))
      return 0;

  } else {

    fprintf (stderr, "%s\n", "Usage: ReplaceIcon <ExeFilename> <IconFilename>");
  }

  return EXIT_FAILURE;
}

