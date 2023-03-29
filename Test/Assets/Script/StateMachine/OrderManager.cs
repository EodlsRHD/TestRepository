using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOrder
{
    void AddObserver(IObserver observer);

    void RemoveObserver(IObserver observer);

    void Nortify(Vector3 newDestination);
}

public interface IObserver
{
    void UpdateOrder(Vector3 newDestination);
}

public class OrderManager : MonoBehaviour, IOrder
{
    private enum eCampType
    {
        Non = -1,
        red,
        blue
    }

    [SerializeField]
    private eCampType _myCamp = eCampType.Non;

    [SerializeField]
    private List<IObserver> _myCharacters = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        _myCharacters.Add(observer);
    }

    public void RemoveObserver(IObserver observer)
    {
        _myCharacters.Remove(observer);
    }

    public void Nortify(Vector3 newDestination)
    {
        foreach(var observer in _myCharacters)
        {
            observer.UpdateOrder(newDestination);
        }
    }

    private void Update()
    {
        Order();
    }

    public void Order()
    {

    }
}
