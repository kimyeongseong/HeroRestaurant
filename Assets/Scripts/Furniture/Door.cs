using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;
using BehaviorDesigner.Runtime;

public class Door : Furniture, IUsable {
    [SerializeField, TabGroup("Door")]
    private DoorData doorData;

    GameObject[] customers = null;

    private Chair[] arrangedChairs = null;

    private BusinessSystem ownerFloorBusinessSystem = null;

    private void Awake()
    {
        onBuildCompleted.AddListener(OnBuildCompleted);
    }

    private void OnDestroy()
    {
        if (CurrentState == FurnitureState.Bought)
        {
            if (OwnerFloor)
                OwnerFloor.onFurnitureListUpdate.RemoveListener(OnFurnitureListUpdate);

            var gameMode = GameMode.Instance;
            if (gameMode)
            {
                gameMode.onBusinessModeStarted.RemoveListener(StartSpawnCustomer);
                gameMode.onEditorModeStarted.RemoveListener(StopSpawnCustomer);
            }
        }
    }

    private void OnBuildCompleted(Furniture furnitrue)
    {
        arrangedChairs = OwnerFloor.GetFurnitures<Chair>();
        OwnerFloor.onFurnitureListUpdate.AddListener(OnFurnitureListUpdate);

        ownerFloorBusinessSystem = OwnerFloor.GetComponent<BusinessSystem>();

        GameMode.Instance.onBusinessModeStarted.AddListener(StartSpawnCustomer);
        GameMode.Instance.onBusinessTimeOvered.AddListener(StopSpawnCustomer);

        customers = Resources.LoadAll<GameObject>("NPC/Customer");
    }

    private IEnumerator CustomerSpawn()
    {
        var waitForSpawnDelay = new WaitForSeconds(doorData.customerSpawnDelay);

        while (true)
        {
            Chair targetChair = null;
            while (targetChair == null)
            {
                yield return null;

                foreach (var chair in arrangedChairs)
                {
                    bool chairAvaliable = chair.CurrentState == FurnitureState.Bought &&
                                          chair.IsUseable &&
                                          chair.LinkedTable != null;

                    if (chairAvaliable)
                    {
                        targetChair = chair;
                        targetChair.IsUseable = false;
                        break;
                    }
                }

            }

            int randomIndex    = Random.Range(0, customers.Length);
            var customerPrepab = customers[randomIndex];
            var customerObj    = Instantiate(customerPrepab, OwnerFloor.transform);
            customerObj.transform.position = transform.position;

            var customer = customerObj.GetComponent<Customer>();
            customer.Setup(OwnerFloor, targetChair);

            ownerFloorBusinessSystem.AddCustomer(customer);

            yield return waitForSpawnDelay;
        }
    }

    private void OnFurnitureListUpdate(Furniture[] furnitures)
    {
        arrangedChairs = OwnerFloor.GetFurnitures<Chair>();
    }

    public void StartSpawnCustomer()
    {
        StartCoroutine("CustomerSpawn");
    }

    public void StopSpawnCustomer()
    {
        StopCoroutine("CustomerSpawn");
    }

    public void Use(GameObject target)
    {
        var customer = target.GetComponent<Customer>();

        Debug.Assert(customer != null, "Door::Use - Customer component not exist in target");

        ownerFloorBusinessSystem.RemoveCustomer(customer);
        Destroy(target);
    }

    public void Unuse(GameObject target)
    {

    }
}
