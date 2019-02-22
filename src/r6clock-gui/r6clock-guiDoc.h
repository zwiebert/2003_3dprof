// r6clock-guiDoc.h : interface of the Cr6clockguiDoc class
//


#pragma once

class Cr6clockguiDoc : public CDocument
{
protected: // create from serialization only
	Cr6clockguiDoc();
	DECLARE_DYNCREATE(Cr6clockguiDoc)

// Attributes
public:

// Operations
public:

// Overrides
	public:
	virtual BOOL OnNewDocument();
	virtual void Serialize(CArchive& ar);

// Implementation
public:
	virtual ~Cr6clockguiDoc();
#ifdef _DEBUG
	virtual void AssertValid() const;
	virtual void Dump(CDumpContext& dc) const;
#endif

protected:

// Generated message map functions
protected:
	DECLARE_MESSAGE_MAP()
};


