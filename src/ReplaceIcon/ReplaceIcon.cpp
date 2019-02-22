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
namespace ReplaceIcon {
  class CIconFile {
  private:
    int      FSize;    // .ico-file size (or FImage size, which is the same)
    PBYTE    FImage;   // holds the entire .ico-file content
    PICONDIR FIconDir;
    int      FCount;   // number of images
    void construct_from_exe(const char *ExeFileName);
    BOOL IsValid();  // invariant

  public:
    CIconFile(const char *FileName); // construct object from an .ico file
    ~CIconFile() { delete FImage; }; // destruct object


    int GetCount() const { return FCount; } // Get number of images
    void GetIconDirEntry(int Index, ICONDIRENTRY &Result); // Get copy of DirEntry by Index
    void IconData(int Index, PBYTE &Data, int &Size); // Get copy of IconData by Index
    BOOL convert (GRPICONDIR *&grpIconDir); // make a GRPICONDIR from our ICONDIR member

    bool save_to_file(const char *filename);
  };

  BOOL ReplaceIcon(const char *ExeFileName, const char *IcoFileName, const char *ResName);
  BOOL ReplaceIcon(const char *ExeFileName, const char *IcoFileName, const char *SrcExeFileName, const char *ResName);
  void DeleteOldIcons(HMODULE hExe, HANDLE hUpdate, const char *ResName);
  BOOL AddNewIcons(HANDLE hUpdate, const char *ResName, WORD Language, CIconFile &IconFile);
  BOOL AddNewIcons(HANDLE hUpdate, const char *ResName, WORD Language, const char *ExeFileName);

  void ExitFailure(const char *Msg);
  //////////////////////////////////////////////////////////////////////////////////////
  // implementation
  //////////////////////////////////////////////////////////////////////////////////////
};
#include <cstring>
#include <cstdio>
#include <cstdlib>
using namespace std;
#include <tchar.h>

namespace ReplaceIcon {

  BOOL
    CIconFile::convert (PGRPICONDIR &grpIconDir)
  {
    int ResourceSize = sizeof (GRPICONDIR) + ((this->GetCount() - 1) * sizeof (GRPICONDIRENTRY));

    PGRPICONDIR GrpIconDir = (PGRPICONDIR) realloc(0, ResourceSize);
    GrpIconDir->idReserved = 0;
    GrpIconDir->idType = 1;
    GrpIconDir->idCount = this->GetCount();

    // build GRPICONDIR from ICONDIR
    // ICONDIRENTRY and GRPICONDIRENTRY are identical, except for the last
    // member (DWORD)dwImageOffset <=> (WORD)nID
    for(int i=0; i < GrpIconDir->idCount; ++i) {
      ICONDIRENTRY IconDirEntry; this->GetIconDirEntry(i, IconDirEntry);
      memmove(&GrpIconDir->idEntries[i], &IconDirEntry, sizeof(GRPICONDIRENTRY));
      GrpIconDir->idEntries[i].nID = i;

    }

    grpIconDir = GrpIconDir;
    return TRUE;
  }

  CIconFile::CIconFile(const char *FileName)
    : FImage(0)
  {
    if (tolower(FileName[strlen(FileName) - 1]) == 'e') {
      construct_from_exe(FileName);
      if (!FImage)
         throw "CIConFIle: constructor failed";
      return;
    }

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
        throw "CIConFIle: constructor failed";
    if (!IsValid())
      throw "CIConFIle: invariant failed. No valid icon resource";
  }

  void
    CIconFile::construct_from_exe(const char *ExeFileName)
  {
    BOOL success = false;
    PICONDIR buf = NULL;
    DWORD off = 0;

    HMODULE hExe = LoadLibraryEx(PCHAR(ExeFileName), 0, LOAD_LIBRARY_AS_DATAFILE);
    if (hExe == NULL)
      ExitFailure("Could not load src exe");

    HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(1), RT_GROUP_ICON);
    for (int i=0; (!hRes) && (i < 0xff); ++i) // TODO:XXX:FIXME:
      hRes = FindResource(hExe, MAKEINTRESOURCE(0x80), RT_GROUP_ICON);
    if (hRes != 0) {
      int ResourceSize = SizeofResource(hExe, hRes);
      HGLOBAL hResLoad = LoadResource(hExe, hRes);
      if (hResLoad == 0)
        ExitFailure("Coult not load resource");

      PGRPICONDIR GrpIconDir = (PGRPICONDIR)LockResource(hResLoad);
      if (GrpIconDir == NULL)
        ExitFailure("Could not lock resource");

      buf = (PICONDIR)realloc((void *)buf, (off += ResourceSize + (2 * GrpIconDir->idCount)));
      memcpy(buf, GrpIconDir, 3 * sizeof(WORD));
      for (int i=0; i < GrpIconDir->idCount; ++i) {
        memcpy(&buf->idEntries[i], &GrpIconDir->idEntries[i], sizeof buf->idEntries[i]);      
      }

      PGRPICONDIR copy_GrpIconDir = PGRPICONDIR(memcpy(new BYTE[ResourceSize], GrpIconDir, ResourceSize));


      for (int i=0; i < GrpIconDir->idCount; ++i) {
        GRPICONDIRENTRY GrpIconDirEntry = GrpIconDir->idEntries[i];
        HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(GrpIconDirEntry.nID), RT_ICON);
        int ResourceSize = SizeofResource(hExe, hRes);
        if (hRes != 0) {
          HGLOBAL hResLoad = LoadResource(hExe, hRes);
          if (hResLoad == 0)
            ExitFailure("Coult not load resource");
          PICONIMAGE icon_image =  (PICONIMAGE)LockResource(hResLoad);

          buf->idEntries[i].dwImageOffset = off;
          buf = (PICONDIR)realloc((void *)buf, (off += ResourceSize));
          memcpy(PBYTE(buf) + buf->idEntries[i].dwImageOffset, icon_image, ResourceSize);
        }
      }

      FreeLibrary(hExe);
      FImage = PBYTE(FIconDir = buf);
      FSize = off;
      FCount = FIconDir->idCount;

    }
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

  bool 
    CIconFile::save_to_file(const char *filename)
  {
    FILE *out = fopen(filename, "wb");
    return (fwrite(PVOID(FImage), 1, FSize, out) == FSize);
  }

  void ExitFailure(const char *Msg)
  {
    fprintf(stderr, "error: %s\n", Msg);

    throw strcpy(new char[strlen(Msg) + 1], Msg);
    exit(EXIT_FAILURE);
  }

  void
    DeleteOldIcons(HMODULE hExe, HANDLE hUpdate, const char *ResName)
  {
    WORD Language = GetIconInfo((HICON)hExe, (PICONINFO)ResName);
    HRSRC hRes = FindResource(hExe, PCHAR(ResName), RT_GROUP_ICON);
    if (hRes != 0) {
      HGLOBAL hResLoad = LoadResource(hExe, hRes);
      if (hResLoad == 0)
        ExitFailure("Coult not load resource");

      PGRPICONDIR GrpIconDir = (PGRPICONDIR)LockResource(hResLoad);
      if (GrpIconDir == NULL)
        ExitFailure("Could not lock resource");

      for (int i=0; i < GrpIconDir->idCount; ++i) {
        GRPICONDIRENTRY GrpIconDirEntry = GrpIconDir->idEntries[i];
        if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(GrpIconDirEntry.nID), Language, NULL, 0))
          ExitFailure("Could not update (delete old) resource");
      }
    }
  }

  BOOL
    AddNewIcons(HANDLE hUpdate, const char *ResName, WORD Language, CIconFile &IconFile)
  {
    BOOL success = false;
    // add new icons
    try {


      PGRPICONDIR GrpIconDir;
      IconFile.convert(GrpIconDir);

      // Add RT_GROUP_ICON resource
      int ResourceSize = sizeof(GRPICONDIR) + (GrpIconDir->idCount - 1) * sizeof(GRPICONDIRENTRY);
      if (!UpdateResource(hUpdate, RT_GROUP_ICON, PCHAR(ResName), Language, GrpIconDir, ResourceSize))
        ExitFailure("Could not update resource");

      // Add RT_ICON resources
      for(int i=0; i < GrpIconDir->idCount; ++i) {
        PBYTE IconData; int IconSize;
        IconFile.IconData(i, IconData, IconSize);

        if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(i), Language, IconData, IconSize))
          ExitFailure("Could not update (add new) resource");
      }


      free(GrpIconDir);
      success = TRUE;
    } catch (...) { }
    return success;
  }

  BOOL
    AddNewIcons(HANDLE hUpdate,
    const char *ResName, WORD Language,
    const char *ExeFileName)
  {
    BOOL success = false;

    HMODULE hExe = LoadLibraryEx(PCHAR(ExeFileName), 0, LOAD_LIBRARY_AS_DATAFILE);
    if (hExe == NULL)
      ExitFailure("Could not load src exe");

    HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(1), RT_GROUP_ICON);
    if (hRes != 0) {
      int ResourceSize = SizeofResource(hExe, hRes);
      HGLOBAL hResLoad = LoadResource(hExe, hRes);
      if (hResLoad == 0)
        ExitFailure("Coult not load resource");

      PGRPICONDIR GrpIconDir = (PGRPICONDIR)LockResource(hResLoad);
      if (GrpIconDir == NULL)
        ExitFailure("Could not lock resource");

      PGRPICONDIR copy_GrpIconDir = PGRPICONDIR(memcpy(new BYTE[ResourceSize], GrpIconDir, ResourceSize));


      // Add RT_GROUP_ICON resource
      if (!UpdateResource(hUpdate, RT_GROUP_ICON, PCHAR(ResName), Language, copy_GrpIconDir, ResourceSize))
        ExitFailure("Could not update resource");


      for (int i=0; i < GrpIconDir->idCount; ++i) {
        GRPICONDIRENTRY GrpIconDirEntry = GrpIconDir->idEntries[i];
        HRSRC hRes = FindResource(hExe, MAKEINTRESOURCE(GrpIconDirEntry.nID), RT_ICON);
        int ResourceSize = SizeofResource(hExe, hRes);
        if (hRes != 0) {
          HGLOBAL hResLoad = LoadResource(hExe, hRes);
          if (hResLoad == 0)
            ExitFailure("Coult not load resource");
          PICONIMAGE icon_image =  (PICONIMAGE)LockResource(hResLoad);
          PICONIMAGE copy_icon_image = PICONIMAGE(memcpy(new BYTE[ResourceSize], icon_image, ResourceSize));

          // Add RT_ICONs
          if (!UpdateResource(hUpdate, RT_ICON, MAKEINTRESOURCE(i), Language, copy_icon_image, ResourceSize))
            ExitFailure("Could not update (add new) resource");
        }
      }

      FreeLibrary(hExe);
      success = TRUE;
    }
    return success;
  }

  BOOL ReplaceIcon(const char *ExeFileName,
    const char *IcoFileName,
    const char *SrcExeFileName,
    const char *ResName)
  {
    BOOL success = false;

    HANDLE hUpdate = BeginUpdateResource(PCHAR(ExeFileName), TRUE);
    if (hUpdate == NULL)
      ExitFailure("Updating resources is not allowed or file does not exists");
    HMODULE hExe = LoadLibraryEx(PCHAR(ExeFileName), 0, LOAD_LIBRARY_AS_DATAFILE);
    if (hExe == NULL)
      ExitFailure("Could not load exe");

    WORD Language = GetIconInfo((HICON)hExe, (PICONINFO)ResName);
    //DeleteOldIcons(hExe, hUpdate, ResName);

    if (IcoFileName) {
      CIconFile IconFile(IcoFileName);
      success = AddNewIcons(hUpdate, ResName, Language, IconFile);
    } else if (SrcExeFileName) {
#if 1
      CIconFile IconFile(SrcExeFileName);
      success = AddNewIcons(hUpdate, ResName, Language, IconFile);
      if (success)
        IconFile.save_to_file("test.ico");
#else
      success = AddNewIcons(hUpdate, ResName, Language, SrcExeFileName);
#endif
    }

    // free exe first and commit resource updates after that
    if (!FreeLibrary(hExe))
      ExitFailure("Could not free executable");
    if (!EndUpdateResource(hUpdate, FALSE))
      ExitFailure("Could not write changes to file");
    return success;
  }




}

int _tmain(int argc, _TCHAR* argv[])
{
  try {
  if (argc == 3) { 
    if (tolower(argv[2][strlen(argv[2]) - 1]) == 'e') {
      if (ReplaceIcon::ReplaceIcon(argv[1], NULL, argv[2], "MAINICON"))
        return 0;
    } else {
      if (ReplaceIcon::ReplaceIcon(argv[1], argv[2],  NULL, "MAINICON"))
        return 0;
    }

  } else if (argc == 2) {
    if (ReplaceIcon::ReplaceIcon(argv[1], NULL, NULL, "MAINICON"))
      return 0;
  } else {

    fprintf (stderr, "%s\n", "Usage: ReplaceIcon <ExeFilename> <IconFilename>");
  }
  } catch (const char *msg) {
    fprintf(stderr, "ReplaceIcon: %s\n", msg);
    return EXIT_FAILURE;
  }

  return EXIT_FAILURE;
}