using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// Button that opens pay page
/// </summary>
public class DonateProductButton : MonoBehaviour
{
    /// <summary>
    /// Draw and buffer product's values
    /// </summary>
    public void Draw(float amount, float price)
    {
        this._amount = amount;
        this._price = price;
    }

    private float _price;
    private float _amount;

    [Header("Prefab's children references")]
    [SerializeField] private Text _priceText;
    [SerializeField] private Text _amountText;

    public void OnClick()
    {
        // todo open url paypage

    }
}
