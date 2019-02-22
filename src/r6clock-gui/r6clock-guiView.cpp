// r6clock-guiView.cpp : implementation of the Cr6clockguiView class
//

#include "stdafx.h"
#include "r6clock-gui.h"

#include "r6clock-guiDoc.h"
#include "r6clock-guiView.h"

#ifdef _DEBUG
#define new DEBUG_NEW
#endif


// Cr6clockguiView

IMPLEMENT_DYNCREATE(Cr6clockguiView, CView)

BEGIN_MESSAGE_MAP(Cr6clockguiView, CView)
	// Standard printing commands
	ON_COMMAND(ID_FILE_PRINT, CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_DIRECT, CView::OnFilePrint)
	ON_COMMAND(ID_FILE_PRINT_PREVIEW, CView::OnFilePrintPreview)
END_MESSAGE_MAP()

// Cr6clockguiView construction/destruction

Cr6clockguiView::Cr6clockguiView()
{
	// TODO: add construction code here

}

Cr6clockguiView::~Cr6clockguiView()
{
}

BOOL Cr6clockguiView::PreCreateWindow(CREATESTRUCT& cs)
{
	// TODO: Modify the Window class or styles here by modifying
	//  the CREATESTRUCT cs

	return CView::PreCreateWindow(cs);
}

// Cr6clockguiView drawing

void Cr6clockguiView::OnDraw(CDC* /*pDC*/)
{
	Cr6clockguiDoc* pDoc = GetDocument();
	ASSERT_VALID(pDoc);

	// TODO: add draw code for native data here
}


// Cr6clockguiView printing

BOOL Cr6clockguiView::OnPreparePrinting(CPrintInfo* pInfo)
{
	// default preparation
	return DoPreparePrinting(pInfo);
}

void Cr6clockguiView::OnBeginPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add extra initialization before printing
}

void Cr6clockguiView::OnEndPrinting(CDC* /*pDC*/, CPrintInfo* /*pInfo*/)
{
	// TODO: add cleanup after printing
}


// Cr6clockguiView diagnostics

#ifdef _DEBUG
void Cr6clockguiView::AssertValid() const
{
	CView::AssertValid();
}

void Cr6clockguiView::Dump(CDumpContext& dc) const
{
	CView::Dump(dc);
}

Cr6clockguiDoc* Cr6clockguiView::GetDocument() const // non-debug version is inline
{
	ASSERT(m_pDocument->IsKindOf(RUNTIME_CLASS(Cr6clockguiDoc)));
	return (Cr6clockguiDoc*)m_pDocument;
}
#endif //_DEBUG


// Cr6clockguiView message handlers
