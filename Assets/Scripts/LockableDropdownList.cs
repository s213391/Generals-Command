using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class LockableDropdownList : MonoBehaviour
{
    Button button;
    GameObject dropdownPanel;
    Button blockoutPanel;
    Image icon;
    TextMeshProUGUI label;
    Image arrow;
    Transform optionsParent;
    Transform originalListParent;

    public int ID { get; private set; }
    public int value { get; private set; }
    public UnityEvent<int, int> onValueChanged;
    public bool interactable;
    public bool[] enabledOptions;

    //set up the dropdown list
    public void Init(int listID)
    {
        blockoutPanel = transform.GetChild(0).GetComponent<Button>();
        dropdownPanel = transform.GetChild(1).gameObject;
        button = transform.GetChild(2).GetComponent<Button>();
        optionsParent = dropdownPanel.transform.GetChild(0);
        originalListParent = transform.parent;

        icon = button.transform.GetChild(0).GetComponent<Image>();
        label = button.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        arrow = button.transform.GetChild(2).GetComponent<Image>();

        ID = listID;
        value = -1;

        enabledOptions = new bool[optionsParent.childCount];
        for (int i = 0; i < optionsParent.childCount; i++)
        {
            enabledOptions[i] = true;
            int j = i;
            Button optionButton = optionsParent.GetChild(i).GetComponent<Button>();
            optionButton.onClick.AddListener(delegate 
            { 
                dropdownPanel.SetActive(false);
                blockoutPanel.gameObject.SetActive(true);
                transform.SetParent(originalListParent, true);
                SetSelectedOption(j);
                onValueChanged.Invoke(ID, value);
            });
        }

        button.onClick.AddListener(delegate 
        {
            bool closing = dropdownPanel.activeInHierarchy;
            dropdownPanel.SetActive(!closing);
            blockoutPanel.gameObject.SetActive(!closing);

            //move the list in hierarchy so that it renders on top
            if (closing)
                transform.SetParent(originalListParent, true);
            else
                transform.SetParent(transform.root, true);
        });

        blockoutPanel.onClick.AddListener(delegate
        {
            dropdownPanel.SetActive(false);
            blockoutPanel.gameObject.SetActive(false);
            transform.SetParent(originalListParent, true);
        });

        dropdownPanel.SetActive(false);
        blockoutPanel.gameObject.SetActive(false);

        SetInteractable(interactable);
        SetOptionsInteractable(enabledOptions);
    }


    //returns the button gameobjects
    public GameObject GetOptionButton(int index)
    {
        return optionsParent.GetChild(index).gameObject;
    }


    //sets the dropdown button to match the selected option
    public void SetSelectedOption(int index)
    {
        if (index == -1)
            return;

        value = index;

        Image selectedIcon = optionsParent.GetChild(value).GetChild(0).gameObject.GetComponent<Image>();
        TextMeshProUGUI selectedLabel = optionsParent.GetChild(value).GetChild(1).gameObject.GetComponent<TextMeshProUGUI>();

        icon.sprite = selectedIcon.sprite;
        icon.color = selectedIcon.color;
        icon.material = selectedIcon.material;

        label.text = selectedLabel.text;
        label.color = selectedLabel.color;
    }

    #region toggleInteraction

    //toggle dropdown list's interactability
    public void SetInteractable(bool interact)
    {
        interactable = interact;
        arrow.gameObject.SetActive(interactable);
        button.interactable = interactable;
    }


    //toggle options' interactability based on enabled status
    public void SetOptionsInteractable(bool[] newEnabledOptions)
    {
        if (newEnabledOptions != null)
            enabledOptions = newEnabledOptions;

        for (int i = 0; i < enabledOptions.Length; i++)
        {
            optionsParent.GetChild(i).GetComponent<Button>().interactable = enabledOptions[i];
            optionsParent.GetChild(i).GetChild(2).gameObject.SetActive(!enabledOptions[i]);
        }
    }

    #endregion
}
