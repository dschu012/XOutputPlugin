
#left joy
if keyboard.getKeyDown(Key.A):
	xoutput[0].lx = xoutput[0].AxisMin 
if keyboard.getKeyDown(Key.S):
	xoutput[0].ly = xoutput[0].AxisMin
if keyboard.getKeyDown(Key.D):
	xoutput[0].lx = xoutput[0].AxisMax
if keyboard.getKeyDown(Key.W):
	xoutput[0].ly = xoutput[0].AxisMax

xoutput[0].rx = mouse.deltaX * 50000
xoutput[0].ry = -(mouse.deltaY * 50000)

xoutput[0].L1 = keyboard.getKeyDown(Key.Space)
#xoutput[0].L2
if mouse.rightButton:
	xoutput[0].R1 = xoutput[0].TriggerMax
if mouse.leftButton:
	xoutput[0].R2 = xoutput[0].TriggerMax
xoutput[0].R3 = keyboard.getKeyDown(Key.E)
xoutput[0].L3 = keyboard.getKeyDown(Key.LeftControl)

xoutput[0].Back = keyboard.getKeyDown(Key.LeftBracket)
xoutput[0].Start = keyboard.getKeyDown(Key.RightBracket)
xoutput[0].Guide = keyboard.getKeyDown(Key.Return)

xoutput[0].A = keyboard.getKeyDown(Key.Q)
xoutput[0].B = keyboard.getKeyDown(Key.R)
xoutput[0].X = keyboard.getKeyDown(Key.F)
xoutput[0].Y = mouse.middleButton

xoutput[0].Up = keyboard.getKeyDown(Key.UpArrow)
xoutput[0].Down = keyboard.getKeyDown(Key.DownArrow)
xoutput[0].Left = keyboard.getKeyDown(Key.LeftArrow)
xoutput[0].Right = keyboard.getKeyDown(Key.RightArrow)