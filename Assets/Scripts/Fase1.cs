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

    public GameObject TimerLabel;
    
    public float time;

    public GameObject Aim;

    private float DistanceIntoTargets = 3f;
    
    private Quaternion defaultQuartenion = Quaternion.Euler(-90, -180, 0);
    
    private Vector3 defaultVector3 = new Vector3(-3f, 4.45f, 0f);
    
    private GameObject[] rules;

    private GameObject[,] symbolRules;

    private GameObject[] headerRules;
    
    private GameObject parentHeaderRules;

    private GameObject currentRule;

    private int currentIndex = -1;

    private int Step = 1;

    private bool setupStep = false;

    private int selectedHeader;

    private GameObject currentReplaceVariable;

    private GameObject currentReplaceVariable2;

    // Start is called before the first frame update
    void Start()
    {
        ParseGramar();
        GrammarVisible.text = GramarToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (startTimer(3))
        {
            return;
        }
        //Step 1 - Selecionar uma regra para ser alterada
        if (Step == 1)
        {
            if(!setupStep)
            {
                Debug.Log("Iniciando Step 1");
                SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = IsSomeHeaderSelected();
            if(indexHeader != -1)
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
        if(Step == 2)
        {
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 2");
                setupStep = true;
            }
            int[] indexRule = IsSomeRuleSelected(selectedHeader);
            if (indexRule[0] != -1)
            {
                if(currentReplaceVariable == null)
                {
                    currentReplaceVariable = symbolRules[selectedHeader, indexRule[0]];
                }
            }
            if(indexRule[1] != -1)
            {
                if (currentReplaceVariable2 == null)
                {
                    currentReplaceVariable2 = symbolRules[selectedHeader, indexRule[1]];
                    Step = 3;
                    setupStep = false;
                    SetRuleInactive();
                    currentIndex = -1;
                    selectedHeader = -1;
                }
            }
        }
        //Step 3 - Selecionar quais as variaveis dentro desta nova variavel
        if(Step == 3)
        {
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 3");
                SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                //SetHeaderRuleInactive();
                //SetRuleActive(indexHeader);
                //selectedHeader = indexHeader;
                //Step = 1;
            }
        }
    }

    void ParseGramar()
    {
        rules = new GameObject[Grammar.Split(',').Length];
        headerRules = new GameObject[Grammar.Split(',').Length];
        symbolRules = new GameObject[4,3];
        var objectHeaderRule = new GameObject();
        objectHeaderRule.name = "TargetHeaders";
        objectHeaderRule.transform.parent = gameObject.transform;
        objectHeaderRule.SetActive(false);
        parentHeaderRules = objectHeaderRule;
        for (int j = 0; j < Grammar.Split(',').Length; j++)
        {
            string[] chars = Grammar.Split(',')[j].Replace(" ", "").Split(new string[] { "->" }, StringSplitOptions.None);
            var objectRule = new GameObject();
            objectRule.transform.parent = gameObject.transform;
            objectRule.name = chars[0];
            objectRule.SetActive(false);
            rules[j] = objectRule;
            
            headerRules[j] = Instantiate(defaultTarget, objectHeaderRule.transform);
            headerRules[j].SetActive(true);
            headerRules[j].transform.rotation = defaultQuartenion;
            headerRules[j].transform.position = new Vector3(defaultVector3.x + (DistanceIntoTargets * j), defaultVector3.y, defaultVector3.z);
            headerRules[j].name = chars[0].ToString();
            headerRules[j].GetComponent<TargetMovimentation>().Symbol = chars[0].ToString();


            for (int i = 0; i < chars[1].Length; i++)
            {
                var clone = Instantiate(defaultTarget, objectRule.transform);
                clone.SetActive(true);
                clone.transform.rotation = defaultQuartenion;
                clone.transform.position = new Vector3(defaultVector3.x+(DistanceIntoTargets * i), defaultVector3.y, defaultVector3.z);
                clone.name = chars[1][i].ToString();
                clone.GetComponent<TargetMovimentation>().Symbol = chars[1][i].ToString();
                symbolRules[j,i] = clone;
            }
        }
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

    int[] IsSomeRuleSelected(int index)
    {
        int[] selected = { -1, -1 };
        int indexSelected = 0;
        for (int i = 0; i < symbolRules.GetUpperBound(index); i++)
        {
            if (symbolRules[index, i].GetComponent<TargetMovimentation>().selected)
            {
                Debug.Log($"A regra selecionada é: {symbolRules[index, i].GetComponent<TargetMovimentation>().name}");
                selected[indexSelected] = i;
                indexSelected++;
            }
        }
        return selected;
    }


    void SetHeaderRuleInactive()
    {
        parentHeaderRules.SetActive(false);
    }

    void SetRuleInactive()
    {
        if(rules[currentIndex])
        {
            rules[currentIndex].SetActive(false);
        }
    }

    string GramarToString()
    {
        string FullGrammar = "";
        string[] text = Grammar.Split(',');
        foreach (var grammar in text)
        {
            FullGrammar = $"{FullGrammar}\n {grammar}";
        }
        return FullGrammar;
    }

    bool startTimer(int maxSeconds = 0)
    {
        
        if (maxSeconds > 0 && (time+Time.deltaTime)%60 > maxSeconds)
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
        Debug.Log($"Time: {time}");
        Debug.Log($"minutes: {minutes}");
        Debug.Log($"seconds: {seconds}");
        Debug.Log($"fraction: {fraction}");
        TimerLabel.GetComponent<Text>().text = $"{Math.Truncate(minutes)}: {Math.Truncate(seconds)}: {Math.Truncate(fraction)}";
        return true;
    }
}
