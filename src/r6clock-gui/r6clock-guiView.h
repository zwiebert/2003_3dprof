// r6clock-guiView.h : interface of the Cr6clockguiView class
//


#pragma once


class Cr6clockguiView : public CView
{
protected: // create from serialization only
	Cr6clockguiView();
	DECLARE_DYNCREATE(Cr6clockguiView)

// Attributes
public:
	Cr6clockguiDoc* GetDocument() const;

// Operations
public:

// Overrides
	public:
	virtual void OnDraw(CDC* pDC);  // overridden to draw this view
virtual BOOL PreCreateWindow(CREATESTRUCT& cs);
protected:
	virtual BOOL OnPreparePrinting(CPrintInfo* pInfo);
	virtual void OnBeginPrinting(CDC* pDC, CPrintInfo* pInfo);
	virtual void OnEndPrinting(CDC* pDC, CPrintInfo* pInfo);

// Implementation
public:
	virtual ~Cr6clockguiView();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};

#ifndef _DEBUG  // debug version in r6clock-guiView.cpp
inline Cr6clockguiDoc* Cr6clockguiView::GetDocument() const
   { return reinterpret_cast<Cr6clockguiDoc*>(m_pDocument); }
#endif

