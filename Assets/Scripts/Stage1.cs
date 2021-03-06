using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Stage1 : MonoBehaviour
{
    public GameObject defaultTarget;

    public string Grammar;

    [SerializeField] Text GrammarVisible;

    [SerializeField] Text GenericText;

    [SerializeField] Text TextStep;

    public GameObject TimerLabel;

    public GameObject Target;

    public float time;

    public GameObject Aim;

    private float DistanceIntoTargets = 3f;

    private Quaternion defaultQuartenion = Quaternion.Euler(-90, -180, 0);

    private Vector3 defaultVector3 = new Vector3(-3f, 3.45f, 0f);
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

    private int Step = 0;

    private bool setupStep = false;

    private int selectedHeader;

    private string newRuleName = "C";

    private GameObject currentReplaceVariable;

    private GameObject currentReplaceVariable2;

    private string currentReplaceString;

    private string currentReplaceString2;

    public GameObject AmmoCounterPannel;

    private AmmunationController AmmunationController;

    public bool TutorialOk = false;

    public GameObject Tutorial;

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

    void DestroyRules()
    {
        if(rules == null)
        {
            return;
        }
        foreach (GameObject rule in rules)
        {
            Destroy(rule);
        }
    }

    void DestroyHeaderRules()
    {
        if (headerRules == null)
        {
            return;
        }
        foreach (GameObject headerRule in headerRules)
        {
            Destroy(headerRule);
        }
    }

    void DestroySymbolRules()
    {
        if (symbolRules == null)
        {
            return;
        }

        foreach (List<GameObject> symbolRule in symbolRules)
        {
            foreach (GameObject rule in symbolRule)
            {
                Destroy(rule);
            }
        }
    }

    void DestroyParentHeaderRules()
    {
        if (parentHeaderRules == null)
        {
            return;
        }
        Destroy(parentHeaderRules);
    }


    void resetGame()
    {
        ParseGramar();
        Target.SetActive(false);
        AmmunationController.GameEnable = false;
        currentReplaceVariable = null;
        currentReplaceVariable2 = null;
        ClearNewRuleText();
        setupStep = false;
        changeStep(0);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateVisibleGramar();
        TextStep.text = $"Etapa: {Step}";
        if (!TutorialOk)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            resetGame();
            return;
        }
        if (Step == 0)
        {
            if (startTimer(3))
            {
                Debug.Log("Esperando timer");
                return;
            }
            Debug.Log("Timer Acabou????");
            Target.SetActive(true);
            AmmunationController.GameEnable = true;
            changeStep(1);
        }
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
                SetHeaderNotSelected();
                SetHeaderActive();
                selectedHeader = indexHeader;
                changeStep(2);
            }
        }
        //Step 2 - Selecionar uma variavel para ser alterada
        if (Step == 2)
        {
            if (!selectHeaderHasMoreThanTwoVariables(selectedHeader))
            {
                //Se não tiver mais de duas variaveis deve voltar para o estágio 1
                changeStep(1);
                Debug.Log("Essa regra ta OK!");
                return;
            }
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 2");
                SetRuleActive(selectedHeader);
                setupStep = true;
                GenericText.text = $"Nova Regra\n {newRuleName} -> ??";
            }
            List<GameObject> selectedObjects = IsSomeRuleSelected(selectedHeader);
            if (selectedObjects.Count > 0)
            {
                if (currentReplaceVariable == null)
                {
                    currentReplaceVariable = selectedObjects[0];
                    GenericText.text = $"Nova Regra\n {newRuleName} -> {currentReplaceVariable.name}?";
                }
            }
            if (selectedObjects.Count > 1)
            {
                currentReplaceVariable2 = selectedObjects[1];
                changeStep(3);
                SetCurrentRuleInactive();
            }
        }
        //Step 3 - Selecionar quais as variaveis dentro desta nova variavel
        if (Step == 3)
        {
            if (IsFirstStepFinished())
            {
                Cursor.visible = true;
                SceneManager.LoadScene("SuccessStage1");
                return;
            }
            if (!setupStep)
            {
                Debug.Log("Iniciando Step 3");
                GenericText.text = $"Nova Regra\n {newRuleName} -> {currentReplaceVariable.name}{currentReplaceVariable2.name}";
                currentReplaceString = currentReplaceVariable.name;
                currentReplaceString2 = currentReplaceVariable2.name;
                replaceTwoRulesByDefaultRule(currentReplaceVariable, currentReplaceVariable2, selectedHeader);
                currentIndex = -1;
                selectedHeader = -1;
                SetHeaderRuleActive();
                setupStep = true;
            }
            if (!FindTwoRulesIntoGrammar(currentReplaceString, currentReplaceString2))
            {
                Debug.Log($"String1: {currentReplaceString}, String2: {currentReplaceString2}");
                Debug.Log("Essa regra não tem as duas variaveis!");
                changeStep(1);
                return;
            }
            int indexHeader = IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                GameObject rule1 = symbolRules[indexHeader].Find(x => x.name == currentReplaceString);
                GameObject rule2 = symbolRules[indexHeader].Find(x => x.name == currentReplaceString2);
                Debug.Log(rule1);
                Debug.Log(rule2);
                if (rule1 != null && rule2 != null)
                {
                    replaceTwoRulesByDefaultRule(rule1, rule2, indexHeader);
                }
                else
                {
                    SetHeaderNotSelected();
                    SetHeaderActive();
                }
            }
        }
    }

    void ClearNewRuleText()
    {
        GenericText.text = "";
        currentReplaceVariable = null;
        currentReplaceVariable2 = null;
        currentReplaceString = "";
        currentReplaceString2 = "";
    }

    void changeStep(int step)
    {
        Debug.Log($"Indo para a etapa: {step}");
        UpdateVisibleGramar();
        SetHeaderNotSelected();
        Step = step;
        setupStep = false;
    }

    void replaceTwoRulesByDefaultRule(GameObject rule1, GameObject rule2, int headerIndex)
    {
        int index = symbolRules[headerIndex].FindIndex(x => x == rule1);
        int index2 = symbolRules[headerIndex].FindIndex(x => x == rule2);
        symbolRules[headerIndex].Insert(
            Math.Min(index, index2),
            CreateDefaultObject(
                rules[headerIndex].transform,
                newRuleName,
                defaultVector3,
                false
            )
        );
        List<GameObject> objectsToDelete = new List<GameObject>();
        objectsToDelete.Add(rule1);
        objectsToDelete.Add(rule2);
        deleteVisibleRule(headerIndex, objectsToDelete);
    }

    bool IsFirstStepFinished()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            if (symbolRules[i].Count > 2)
            {
                return false;
            }
        }
        return true;
    }

    void ParseGramar()
    {
        DestroyParentHeaderRules();
        DestroyRules();
        DestroyHeaderRules();
        DestroySymbolRules();
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

    void SetRuleInactive()
    {
        foreach (GameObject rule in rules)
        {
            rule.SetActive(false);
        }

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
    bool selectHeaderHasMoreThanTwoVariables(int index)
    {
        if (symbolRules[index].Count < 3)
        {
            return false;
        }
        int qtdVariables = 0;
        foreach (GameObject rule in symbolRules[index])
        {
            if (Char.IsUpper(rule.name[0]))
            {
                qtdVariables++;
                if (qtdVariables > 2)
                {
                    return true;
                }
            }
        }
        return false;
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

    void SetCurrentRuleInactive()
    {
        if (rules[currentIndex])
        {
            rules[currentIndex].SetActive(false);
        }
    }

    void SetAllTargetRulesActive()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                rule.SetActive(true);
            }
        }
    }

    void SetAllTargetRulesNotSelected()
    {
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                rule.GetComponent<TargetMovimentation>().selected = false;
            }
        }
    }

    bool FindTwoRulesIntoGrammar(string rule1, string rule2)
    {
        GameObject lastRule = null;
        GameObject currentRule = null;
        for (int i = 0; i < headerRules.Length; i++)
        {
            foreach (GameObject rule in symbolRules[i])
            {
                lastRule = currentRule;
                currentRule = rule;
                if (lastRule != null && currentRule != null)
                {
                    if (lastRule.name == rule1 && currentRule.name == rule2)
                    {
                        return true;
                    }
                }
            }
            lastRule = null;
            currentRule = null;
        }
        return false;
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
            time = 0;
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

    public void FinishTutorial()
    {
        TutorialOk = true;
        Cursor.visible = false;
        Tutorial.SetActive(false);
    }
}
