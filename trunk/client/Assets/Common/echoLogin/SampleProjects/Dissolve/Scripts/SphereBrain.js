#pragma strict

// example to show how easy it is to start an effect 
class SphereBrain extends EchoComponent
{
	function Start () 
	{
		var efx:EchoFXEvent;
		
		EchoFXEvent.Scroll_echoUV ( this, Vector4 ( 0.01, 0, 0.02, 0 ), 0 );
		
		efx = EchoFXEvent.Dissolve_echoShader ( this, -0.5, 1.5, 2.0 );
		efx.StartDelay ( 2.0, null );

		efx = EchoFXEvent.Dissolve_echoShader ( this, 1.5, -0.5, 2.0 );
		efx.StartDelay ( 5.0, null );
		
	}
	
}
