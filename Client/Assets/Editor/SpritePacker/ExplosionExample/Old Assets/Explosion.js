//#pragma strict





private var directions = Array(Vector3(0, 0, 1), Vector3(0, 0, -1), Vector3(1, 0, 0), Vector3(-1, 0, 0), Vector3(0, 1, 0), Vector3(0, -1, 0));
private var hit : RaycastHit;

		var toCam = (Camera.main.transform.position - transform.position).normalized * 1.35;
		var dirIndex = 0;
			var random = Random.onUnitSphere;
			
			if(dirIndex >= directions.length) dirIndex = 0;
			var dir : Vector3 = directions[dirIndex];
			
			dirIndex ++;
}

function Update ()