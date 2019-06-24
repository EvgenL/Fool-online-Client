using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddMoneyContainer : MonoBehaviour
{
    [SerializeField] private Text _errorText;
    [SerializeField] private InputField _sumField;
    [SerializeField] private Payment _paymentManager;

    public float MinimumSum = 50.0f; // todo store on server, get value from server
    public float MaximumSum = 10000.0f; // todo store on server, get value from server

    private void Awake()
    {
        _errorText.text = "";
    }

    public void OnSendClick()
    {
        _errorText.text = "";

        float sum;
        if (!float.TryParse(_sumField.text, out sum))
        {
            _errorText.text = "Неверный ввод";
            return;
        }

        if (sum < MinimumSum)
        {
            _errorText.text = "Сумма не должна быть меньше " + MinimumSum;
            return;
        }

        if (sum > MaximumSum)
        {
            _errorText.text = "Сумма не должна быть больше " + MaximumSum;
            return;
        }

        _paymentManager.AddMoney(sum);

    }

}
