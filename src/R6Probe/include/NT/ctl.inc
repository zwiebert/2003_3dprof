; ---------------------------------------------------------
;  IOCTLs
; ---------------------------------------------------------

CTL_FUNCTION_MASK   EQU 03ffch
CTL_FUNCTION_SHIFT  EQU 2

CTL_CODE MACRO lbl, DeviceType, Function, _Method, Access
         lbl = (DeviceType SHL 16) OR (Access SHL 14) OR (Function SHL 2) OR _Method
         ENDM

METHOD_BUFFERED     EQU 0
METHOD_IN_DIRECT    EQU 1
METHOD_OUT_DIRECT   EQU 2
METHOD_NEITHER      EQU 3

FILE_ANY_ACCESS     EQU 0
FILE_SPECIAL_ACCESS EQU 0
FILE_READ_ACCESS    EQU 1
FILE_WRITE_ACCESS   EQU 2

