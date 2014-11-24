
using UnityEngine;
using System.Collections;

public class NatTester : MonoBehaviour
{
    // This script runs the connection tests we need to run.

    private ConnectionTesterStatus natCapable = ConnectionTesterStatus.Undetermined;
    public static bool filterNATHosts = false;
    private bool probingPublicIP = false;
    private bool doneTestingNAT = false;
    private float timer = 0.0f;

    private bool hideTest = false;
    private string testMessage = "Undetermined NAT capabilities";

    public IEnumerator Start()
    {

        // Start connection test
        natCapable = Network.TestConnection();

        yield return 0;

        if (Network.HavePublicAddress())
        {
            Debug.Log("This machine has a public IP address");
        }
        else
        {
            Debug.Log("This machine has a private IP address");
        }
    }

    private bool testing = false;

    public void Update()
    {
        if (!doneTestingNAT && !testing)
        {
            StartCoroutine(TestConnection());
        }

    }


    public IEnumerator TestConnection()
    {
        // Start/Poll the connection test, report the results in a label and react to the results accordingly
        testing = true;

        natCapable = Network.TestConnection();
        yield return new WaitForSeconds(0.5f);

        switch (natCapable)
        {
            case ConnectionTesterStatus.Error:
                {
                    testMessage = "Problem determining NAT capabilities";
                    doneTestingNAT = true;
                }
                break;

            case ConnectionTesterStatus.Undetermined:
                {
                    testMessage = "Undetermined NAT capabilities";
                    doneTestingNAT = false;
                }
                break;

            case ConnectionTesterStatus.PrivateIPNoNATPunchthrough:
                {
                    testMessage = "Cannot do NAT punchthrough, filtering NAT enabled hosts for client connections, local LAN games only.";
                    filterNATHosts = true;

                    doneTestingNAT = true;
                }
                break;

            case ConnectionTesterStatus.PrivateIPHasNATPunchThrough:
                {
                    if (probingPublicIP)
                        testMessage = "Non-connectable public IP address (port  blocked), NAT punchthrough can circumvent the firewall.";
                    else
                        testMessage = "NAT punchthrough capable. Enabling NAT punchthrough functionality.";
                    // NAT functionality is enabled in case a server is started,
                    // clients should enable this based on if the host requires it

                    doneTestingNAT = true;
                }
                break;

            case ConnectionTesterStatus.PublicIPIsConnectable:
                {
                    testMessage = "Directly connectable public IP address.";

                    doneTestingNAT = true;
                }
                break;

            // This case is a bit special as we now need to check if we can 
            // cicrumvent the blocking by using NAT punchthrough
            case ConnectionTesterStatus.PublicIPPortBlocked:
                {
                    testMessage = "Non-connectble public IP address (port  blocked), running a server is impossible.";

                    // If no NAT punchthrough test has been performed on this public IP, force a test
                    if (!probingPublicIP)
                    {
                        Debug.Log("Testing if firewall can be circumnvented");
                        natCapable = Network.TestConnectionNAT();
                        probingPublicIP = true;
                        timer = Time.time + 10;
                    }
                    // NAT punchthrough test was performed but we still get blocked
                    else if (Time.time > timer)
                    {
                        probingPublicIP = false; 		// reset

                        doneTestingNAT = true;
                    }
                }
                break;
            case ConnectionTesterStatus.PublicIPNoServerStarted:
                {
                    testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
                }
                break;
            default:
                {
                    testMessage = "Error in test routine, got " + natCapable;
                }
                break;
        }
        Debug.Log(testMessage);


        if (doneTestingNAT)
        {
            Debug.Log("TestConn:" + testMessage);
            Debug.Log("TestConn:" + natCapable + " " + probingPublicIP + " " + doneTestingNAT);
        }
        testing = false;
    }

}