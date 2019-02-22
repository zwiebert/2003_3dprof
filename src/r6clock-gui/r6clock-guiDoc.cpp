// r6clock-guiDoc.cpp : implementation of the Cr6clockguiDoc class
//

#include "stdafx.h"
#include "r6clock-gui.h"

#include "r6clock-guiDoc.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// Cr6clockguiDoc

IMPLEMENT_DYNCREATE(Cr6clockguiDoc, CDocument)

BEGIN_MESSAGE_MAP(Cr6clockguiDoc, CDocument)
END_MESSAGE_MAP()


// Cr6clockguiDoc construction/destruction

Cr6clockguiDoc::Cr6clockguiDoc()
{
	// TODO: add one-time construction code here

}

Cr6clockguiDoc::~Cr6clockguiDoc()
{
}

BOOL Cr6clockguiDoc::OnNewDocument()
{
	if (!CDocument::OnNewDocument())
		return FALSE;

	// TODO: add reinitialization code here
	// (SDI documents will reuse this document)

	return TRUE;
}




// Cr6clockguiDoc serialization

void Cr6clockguiDoc::Serialize(CArchive& ar)
{
	if (ar.IsStoring())
	{
		// TODO: add storing code here
	}
	else
	{
		// TODO: add loading code here
	}
}


// Cr6clockguiDoc diagnostics

#ifdef _DEBUG
void Cr6clockguiDoc::AssertValid() const
{
	CDocument::AssertValid();
}

void Cr6clockguiDoc::Dump(CDumpContext& dc) const
{
	CDocument::Dump(dc);
}
#endif //_DEBUG


// Cr6clockguiDoc commands
