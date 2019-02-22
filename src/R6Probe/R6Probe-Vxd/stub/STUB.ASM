.model tiny
.code
;        org 0
;        db  20h dup(0)
start:
;        push cs
;        pop  ds
;        mov  dx, offset msg
;        mov  ah, 9
;        int  21h
        mov  ax, 4c00h
        int  21h        

         db  (20h - 5) dup(0)
;msg:    db   "This program cannot be run in DOS mode",0dh,0ah,'$'

end start
