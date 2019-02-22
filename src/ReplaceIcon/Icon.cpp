#include <windows.h>
//#include "Icon.h"
#if 0
/****************************************************************************
*
*     FUNCTION: ReadIconFromEXEFile
*
*     PURPOSE:  Load an Icon Resource from a DLL/EXE file
*
*     PARAMS:   LPCTSTR szFileName - name of DLL/EXE file
*
*     RETURNS:  LPICONRESOURCE - pointer to icon resource
*
* History:
*                July '95 - Created
*
\****************************************************************************/
LPICONRESOURCE ReadIconFromEXEFile( LPCTSTR szFileName )
{
    LPICONRESOURCE    	lpIR = NULL, lpNew = NULL;
    HINSTANCE        	hLibrary;
    LPTSTR            	lpID;
    EXEDLLICONINFO    	EDII;

    // Load the DLL/EXE - NOTE: must be a 32bit EXE/DLL for this to work
    if( (hLibrary = LoadLibraryEx( szFileName, NULL, LOAD_LIBRARY_AS_DATAFILE )) == NULL )
    {
        // Failed to load - abort
        MessageBox( hWndMain, "Error Loading File - Choose a 32bit DLL or EXE", szFileName, MB_OK );
        return NULL;
    }
    // Store the info
    EDII.szFileName = szFileName;
    EDII.hInstance = hLibrary;
    // Ask the user, "Which Icon?"
    if( (lpID = ChooseIconFromEXEFile( &EDII )) != NULL )
    {
        HRSRC        	hRsrc = NULL;
        HGLOBAL        	hGlobal = NULL;
        LPMEMICONDIR    lpIcon = NULL;
        UINT            i;

        // Find the group icon resource
        if( (hRsrc = FindResource( hLibrary, lpID, RT_GROUP_ICON )) == NULL )
        {
            FreeLibrary( hLibrary );
            return NULL;
        }
        if( (hGlobal = LoadResource( hLibrary, hRsrc )) == NULL )
        {
            FreeLibrary( hLibrary );
            return NULL;
        }
        if( (lpIcon = LockResource(hGlobal)) == NULL )
        {
            FreeLibrary( hLibrary );
            return NULL;
        }
        // Allocate enough memory for the images
        if( (lpIR = malloc( sizeof(ICONRESOURCE) + ((lpIcon->idCount-1) * sizeof(ICONIMAGE)) )) == NULL )
        {
            MessageBox( hWndMain, "Error Allocating Memory", szFileName, MB_OK );
            FreeLibrary( hLibrary );
            return NULL;
        }
        // Fill in local struct members
        lpIR->nNumImages = lpIcon->idCount;
        lstrcpy( lpIR->szOriginalDLLFileName, szFileName );
        lstrcpy( lpIR->szOriginalICOFileName, "" );
        // Loop through the images
        for( i = 0; i < lpIR->nNumImages; i++ )
        {
            // Get the individual image
            if( (hRsrc = FindResource( hLibrary, MAKEINTRESOURCE(lpIcon->idEntries[i].nID), RT_ICON )) == NULL )
            {
                free( lpIR );
                FreeLibrary( hLibrary );
                return NULL;
            }
            if( (hGlobal = LoadResource( hLibrary, hRsrc )) == NULL )
            {
                free( lpIR );
                FreeLibrary( hLibrary );
                return NULL;
            }
            // Store a copy of the resource locally
            lpIR->IconImages[i].dwNumBytes = SizeofResource( hLibrary, hRsrc );
            lpIR->IconImages[i].lpBits = malloc( lpIR->IconImages[i].dwNumBytes );
            memcpy( lpIR->IconImages[i].lpBits, LockResource( hGlobal ), lpIR->IconImages[i].dwNumBytes );
            // Adjust internal pointers
            if( ! AdjustIconImagePointers( &(lpIR->IconImages[i]) ) )
            {
                MessageBox( hWndMain, "Error Converting to Internal Format", szFileName, MB_OK );
                free( lpIR );
                FreeLibrary( hLibrary );
                return NULL;
            }
        }
    }
    FreeLibrary( hLibrary );
    return lpIR;
}
/* End ReadIconFromEXEFile() ************************************************/
#endif