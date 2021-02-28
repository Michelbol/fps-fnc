using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Fase1 : MonoBehaviour
{
    public GameObject defaultTarget;

    public string Grammar;

    [SerializeField] Text GrammarVisible;

    [SerializeField] Text GenericText;

    public GameObject TimerLabel;

    public float time;

    public GameObject Aim;

    private float DistanceIntoTargets = 3f;

    private Quaternion defaultQuartenion = Quaternion.Euler(-90, -180, 0);

    private Vector3 defaultVector3 = new Vector3(-3f, 4.45f, 0f);
    // Object into Fase with all rules, used to select when hit into a header
    private GameObject[] rules;
    // Inside of rules has N symbolRules is the terminal symbol
    private List<List<GameObject>> symbolRules;
    // Is Game Object of headers to hit
    private GameObject[] headerRules;
    // Game Object having all header rules
    private GameObject parentHeaderRules;
    // Symbol not terminal(header rule) selected 
    private GameObject currentRule;
    // Index of symbol not terminal(header rule) selected
    private int currentIndex = -1;

    private int Step = 1;

    private bool setupStep = false;

    private int selectedHeader;

    private string newRuleName = "C";

    private GameObject currentReplaceVariable;

    private GameObject currentReplaceVariable2;

    public GameObject AmmoCounterPannel;

    private AmmunationController AmmunationController;

    // Start is called before the first frame update
    void Start()
    {
        AmmunationController = AmmoCounterPannel.GetComponent<AmmunationController>();
        ParseGramar();
        UpdateVisibleGramar();
        AmmunationController.GameEnable = false;
    }

    void UpdateVisibleGramar()
    {
        GrammarVisible.text = GramarToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer(3))
        {
            return;
        }
        AmmunationController.GameEnable = true;
        //Step 1 - Selecionar uma regra para ser alterada
        if (Step == 1)
        {
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 1");
                SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                SetHeaderRuleInactive();
                SetRuleActive(indexHeader);
                SetHeaderNotSelected();
                SetHeaderActive();
                selectedHeader = indexHeader;
                Step = 2;
                setupStep = false;
            }
        }
        //Step 2 - Selecionar uma variavel para ser alterada
        if (Step == 2)
        {
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 2");
                setupStep = true;
            }
            List<GameObject> selectedObjects = IsSomeRuleSelected(selectedHeader);
            if (selectedObjects.Count > 0)
            {
                if (currentReplaceVariable == null)
                {
                    currentReplaceVariable = selectedObjects[0];
                }
            }
            if (selectedObjects.Count > 1)
            {
                currentReplaceVariable2 = selectedObjects[1];
                Step = 3;
                setupStep = false;
                SetRuleInactive();
                //currentIndex = -1;
                //selectedHeader = -1;
            }
        }
        //Step 3 - Selecionar quais as variaveis dentro desta nova variavel
        if (Step == 3)
        {
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 3");
                GenericText.text = $"Nova Regra\n {newRuleName} -> {currentReplaceVariable.name}{currentReplaceVariable2.name}";
                symbolRules[selectedHeader].Add(
                    CreateDefaultObject(
                        currentReplaceVariable.transform,
                        newRuleName,
                        defaultVector3,
                        false
                    )
                );
                List<GameObject> objectsToDelete = new List<GameObject>();
                objectsToDelete.Add(currentReplaceVariable);
                objectsToDelete.Add(currentReplaceVariable2);
                deleteVisibleRule(selectedHeader, objectsToDelete);
                currentIndex = -1;
                selectedHeader = -1;
                SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                //for(int i = 0; i < symbolRules.Length; i++)
                //{
                //   for (int j = 0; j < symbolRules.GetUpperBound(i); j++)
                //  {
                //     if (symbolRules[i,j].name == currentReplaceVariable.name && (j+1) < symbolRules.GetUpperBound(i) && symbolRules[i, j+1].name == currentReplaceVariable.name)
                //    {
                //       Debug.Log("Da pra ");
                //  }
                //}
                //}
            }
        }
    }

    void ParseGramar()
    {
        var gramarSplitComma = Grammar.Split(',');
        rules = new GameObject[gramarSplitComma.Length];
        headerRules = new GameObject[gramarSplitComma.Length];
        symbolRules = new List<List<GameObject>>();
        parentHeaderRules = CreateEmptyObject(
            gameObject.transform,
            "TargetHeaders",
            false
        );
        for (int j = 0; j < gramarSplitComma.Length; j++)
        {
            string[] chars = gramarSplitComma[j].Replace(" ", "").Split(new string[] { "->" }, StringSplitOptions.None);

            rules[j] = CreateEmptyObject(transform, chars[0], false);

            headerRules[j] = CreateDefaultObject(
                parentHeaderRules.transform,
                chars[0].ToString(),
                new Vector3(defaultVector3.x + (DistanceIntoTargets * j), defaultVector3.y, defaultVector3.z)
                );

            List<GameObject> ruleJ = new List<GameObject>();

            for (int i = 0; i < chars[1].Length; i++)
            {
                ruleJ.Add(
                    CreateDefaultObject(
                    rules[j].transform,
                    chars[1][i].ToString(),
                    new Vector3(
                        defaultVector3.x + (DistanceIntoTargets * i),
                        defaultVector3.y,
                        defaultVector3.z
                        )
                    )
                );
            }
            symbolRules.Add(ruleJ);
        }
    }

    GameObject CreateDefaultObject(Transform transform, string name, Vector3 position, bool active = true)
    {
        var obj = Instantiate(defaultTarget, transform);
        obj.SetActive(active);
        obj.transform.rotation = defaultQuartenion;
        obj.transform.position = position;
        obj.name = name;
        obj.GetComponent<TargetMovimentation>().Symbol = name;
        return obj;
    }

    GameObject CreateEmptyObject(Transform transform, string name, bool active = true)
    {
        var obj = new GameObject();
        obj.transform.parent = transform;
        obj.name = name;
        obj.SetActive(active);
        return obj;
    }

    void deleteVisibleRule(int indexRule, List<GameObject> rules)
    {
        foreach (GameObject rule in rules)
        {
            GameObject findRule = symbolRules[indexRule].Find(x => x == rule);
            Destroy(findRule);
            symbolRules[indexRule].Remove(rule);
        }

        FixDistanceRule(indexRule);
        UpdateVisibleGramar();
    }

    void SetRuleActive(int index)
    {
        rules[index].SetActive(true);
        currentIndex = index;
        currentRule = rules[index];
    }

    void SetHeaderRuleActive()
    {
        parentHeaderRules.SetActive(true);
    }

    int IsSomeHeaderSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            if (headerRules[i].GetComponent<TargetMovimentation>().selected)
            {
                return i;
            }
        }
        return -1;
    }

    void SetHeaderNotSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            headerRules[i].GetComponent<TargetMovimentation>().selected = false;
        }
    }

    void SetHeaderActive()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            headerRules[i].SetActive(true);
        }
    }

    void FixDistanceRule(int index)
    {
        int i = 1;
        foreach (GameObject rule in symbolRules[index])
        {
            rule.transform.position = new Vector3(defaultVector3.x + (DistanceIntoTargets * i), defaultVector3.y, defaultVector3.z);
            i++;
        }
    }

    List<GameObject> IsSomeRuleSelected(int index)
    {
        List<GameObject> selectedRules = new List<GameObject>();
        foreach (GameObject rule in symbolRules[index])
        {
            if (rule.GetComponent<TargetMovimentation>().selected)
            {
                selectedRules.Add(rule);
            }
        }
        return selectedRules;
    }


    void SetHeaderRuleInactive()
    {
        parentHeaderRules.SetActive(false);
    }

    void SetRuleInactive()
    {
        if (rules[currentIndex])
        {
            rules[currentIndex].SetActive(false);
        }
    }

    string GramarToString()
    {
        string FullGrammar = "Gramática";
        for (int i = 0; i < headerRules.Length; i++)
        {
            FullGrammar = $"{FullGrammar}\n {headerRules[i].name} -> ";
            foreach (GameObject rule in symbolRules[i])
            {
                FullGrammar = $"{FullGrammar}{rule.name}";
            }
        }
        return FullGrammar;
    }

    bool startTimer(int maxSeconds = 0)
    {

        if (maxSeconds > 0 && (time + Time.deltaTime) % 60 > maxSeconds)
        {
            Aim.SetActive(true);
            TimerLabel.SetActive(false);
            return false;
        }

        Aim.SetActive(false);
        TimerLabel.SetActive(true);
        time += Time.deltaTime;

        var minutes = time / 60;
        var seconds = time % 60;
        var fraction = (time * 100) % 1000;
        TimerLabel.GetComponent<Text>().text = $"{Math.Truncate(minutes)}: {Math.Truncate(seconds)}: {Math.Truncate(fraction)}";
        return true;
    }
}
