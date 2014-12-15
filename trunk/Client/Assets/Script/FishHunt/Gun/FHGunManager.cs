using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FHGunManager : SingletonMono<FHGunManager>
{
    public Camera GunCamera;
    private Dictionary<string, FHGun> guns = new Dictionary<string, FHGun>();

    public FHGun SpawnGun(ConfigGunRecord config, FHPlayerController controller)
    {
        string name = controller.name + config.id.ToString();
        if (guns.ContainsKey(name))
        {
            guns[name].transform.localPosition = Vector3.zero;
            return guns[name];
        }

        FHGun gun = ((GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Gun/" + config.name, typeof(GameObject)))).GetComponent<FHGun>();

        gun.name = config.name;
        
        gun.transform.parent = controller.gunAnchor.transform;
        gun.transform.localPosition = Vector3.zero;
        gun.transform.localRotation = Quaternion.identity;
        gun.transform.localScale = Vector3.one;
        
        gun.Setup(controller, config);

        guns[name] = gun;

        return gun;
    }

    public void DespawnGun(FHGun gun)
    {
        gun.SetState(GunState.Idle);

        Vector3 localPos = gun.transform.localPosition;
        gun.transform.localPosition = new Vector3(localPos.x, -999999.0f, localPos.z);
    }
}