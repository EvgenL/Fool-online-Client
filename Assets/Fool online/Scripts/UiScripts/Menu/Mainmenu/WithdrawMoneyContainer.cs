using System.Collections;
using System.Collections.Generic;
using Fool_online.Scripts.FoolNetworkScripts;
using UnityEngine;
using UnityEngine.UI;

public class WithdrawMoneyContainer : MonoBehaviour
{
    [SerializeField] private Text _errorText;
    [SerializeField] private InputField _sumField;
    [SerializeField] private InputField _requisitesField;
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

        if (sum < MinimumSum)
        {
            _errorText.text = "Сумма не должна быть больше " + MaximumSum;
            return;
        }

        if (sum > FoolNetwork.LocalPlayer.Money)
        {
            _errorText.text = "У вас недостаточно денег на счету";
            return;
        }

        if (_requisitesField.text.Length < 3)
        {
            _errorText.text = "Введите реквезиты";
            return;
        }

        // Показываем диалог подтверждения "да/нет", который после нажатия "да" отправит запрос на сервер для вывода денег.
        DialogueManager.Instance.ShowYesNo("Пожайлуйста, убедитесь, что верно ввели реквезиты.\n" +
                                           "Платежи обрабатываются каждую пятницу.", "Верно", "Отмена",
            () =>
            {
                _paymentManager.WithdtawMoney(sum, _requisitesField.text);
                DialogueManager.Instance.ShowOk("Запрос успешно отправлен.");
            });
        
    }
}
