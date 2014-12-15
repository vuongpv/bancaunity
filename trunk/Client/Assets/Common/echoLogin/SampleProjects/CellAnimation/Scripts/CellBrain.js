#pragma strict

// example of cell animation on a polygon face
class CellBrain extends EchoComponent
{
	function Start () 
	{
		EchoFXEvent.CellAnimate_echoUV ( this, ELC.CELL4, ELC.CELL4, 4, 0, 16, 0.15, 0 );
	}
	
}
