using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class Stage1 : MonoBehaviour
{

    [SerializeField] Text GenericText;

    [SerializeField] Text Description;

    [SerializeField] Text TextStep;

    public GameObject TimerLabel;

    public GameObject Target;

    public float time;

    public GameObject Aim;

    // Symbol not terminal(header rule) selected 
    private GameObject currentRule;
    // Index of symbol not terminal(header rule) selected
    private int currentIndex = -1;

    private int Step = 0;

    private bool setupStep = false;

    private int selectedHeader;

    private GameObject currentReplaceVariable;

    private GameObject currentReplaceVariable2;

    private string currentReplaceString;

    private string currentReplaceString2;

    private string TutorialTitle = "Fase 1";

    public GameObject TutorialObjectTitle;

    private string TutorialRule = "- Cada Regra deve ter apenas 2 variáveis\n- Ou apenas 1 símbolo terminal";
    
    public GameObject TutorialObjectRule;

    private string TutorialHow = "- Para substituir, acerte os alvos!";

    public GameObject TutorialObjectHow;

    private string TutorialReload = "- Deu ruim? Recarregue para reiniciar (R)";
    
    public GameObject TutorialObjectReload;

    public GameObject AmmoCounterPannel;

    public GameObject alert;

    private AmmunationController AmmunationController;

    private AlertController alertController;

    public bool TutorialOk = false;

    public GameObject Tutorial;

    public Grammar grammar;

    private Utils utils = new Utils();

    // Start is called before the first frame update
    void Start()
    {
        TutorialObjectTitle.GetComponent<Text>().text = TutorialTitle;
        TutorialObjectRule.GetComponent<Text>().text = TutorialRule;
        TutorialObjectHow.GetComponent<Text>().text = TutorialHow;
        TutorialObjectReload.GetComponent<Text>().text = TutorialReload;

        AmmunationController = AmmoCounterPannel.GetComponent<AmmunationController>();
        alertController = alert.GetComponent<AlertController>();
        grammar.ParseGramar();
        grammar.UpdateVisibleGramar();
        AmmunationController.GameEnable = false;
    }


    void resetGame()
    {
        grammar.ParseGramar();
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
        grammar.UpdateVisibleGramar();
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
            GameObject[] activeObjects = new GameObject[1];
            activeObjects[0] = Aim;

            GameObject[] inactiveObjects = new GameObject[0];

            if (utils.startTimer(activeObjects, inactiveObjects, TimerLabel, 3))
            {
                return;
            }
            Target.SetActive(true);
            AmmunationController.GameEnable = true;
            changeStep(1);
        }
        //Step 1 - Selecionar uma regra para ser alterada
        if (Step == 1)
        {
            if (!setupStep)
            {
                grammar.SetHeaderRuleActive();
                Description.text = "Atire no Alvo que tem mais de Duas Letras Maiúsculas.";
                setupStep = true;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                if (!grammar.selectHeaderHasMoreThanTwoVariables(indexHeader))
                {
                    //AVISO: Gramática não pode ser substituida
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                    alertController.startTimer("Esse Alvo não tem mais de duas letras maiúsculas!");
                    return;
                }
                selectedHeader = indexHeader;
                grammar.SetHeaderRuleInactive();
                grammar.SetHeaderNotSelected();
                grammar.SetHeaderActive();
                changeStep(2);
            }
        }
        //Step 2 - Selecionar uma variavel para ser alterada
        if (Step == 2)
        {
            if (!setupStep)
            {
                currentRule = grammar.SetRuleActive(selectedHeader);
                currentIndex = selectedHeader;
                setupStep = true;
                Description.text = "Atire em duas Letras maiúsculas para substituir.";
                GenericText.text = $"Nova Regra\n {grammar.newRuleName} -> ??";
            }
            List<GameObject> selectedObjects = grammar.IsSomeRuleSelected(selectedHeader);
            if (selectedObjects.Count > 0)
            {
                if (currentReplaceVariable == null)
                {
                    currentReplaceVariable = selectedObjects[0];
                    Description.text = "Selecione a segunda letra maiúscula para ser substituida.";
                    GenericText.text = $"Nova Regra\n {grammar.newRuleName} -> {currentReplaceVariable.name}?";
                }
            }
            if (selectedObjects.Count > 1)
            {
                currentReplaceVariable2 = selectedObjects[1];
                if (grammar.CountTwoRulesIntoGrammar(currentReplaceVariable.name, currentReplaceVariable2.name) <= 1)
                {
                    //Aviso: Esses simbolos não podem ser substituidos
                    alertController.startTimer("Não existe essas letras em outras regras");
                    grammar.SetTargetRulesActive(selectedHeader);
                    grammar.SetRuleNotSelected(selectedHeader);
                    grammar.SetTargetRulesActiveAndNotSelectedAndFixPosition(selectedHeader);
                    currentReplaceVariable = null;
                    currentReplaceVariable2 = null;
                    Description.text = "";
                    GenericText.text = "";
                    grammar.SetCurrentRuleInactive(currentIndex);
                    changeStep(1);
                    return;
                }
                changeStep(3);
                grammar.SetCurrentRuleInactive(currentIndex);
            }
        }
        //Step 3 - Selecionar quais as variaveis dentro desta nova variavel
        if (Step == 3)
        {
            if (IsFirstStepFinished())
            {
                alertController.successMessage("Parabéns!! Você conseguiu finalizar a primeira etapa!");
                if (utils.setTimeOut(3))
                {
                    return;
                }
                Cursor.visible = true;
                SceneManager.LoadScene("SuccessStage1");
                return;
            }
            if (!setupStep)
            {
                Description.text = "Atire em outras regras que tenham as duas letras .";
                GenericText.text = $"Nova Regra\n {grammar.newRuleName} -> {currentReplaceVariable.name}{currentReplaceVariable2.name}";
                currentReplaceString = currentReplaceVariable.name;
                currentReplaceString2 = currentReplaceVariable2.name;
                grammar.replaceTwoRulesByDefaultRule(currentReplaceVariable, currentReplaceVariable2, selectedHeader);
                currentIndex = -1;
                selectedHeader = -1;
                grammar.SetHeaderRuleActive();
                setupStep = true;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                GameObject rule1 = grammar.FindSymbolRulesByHeaderAndName(indexHeader, currentReplaceString);
                GameObject rule2 = grammar.FindSymbolRulesByHeaderAndName(indexHeader, currentReplaceString2);
                if (rule1 != null && rule2 != null)
                {
                    grammar.replaceTwoRulesByDefaultRule(rule1, rule2, indexHeader);
                }
                else
                {
                    alertController.startTimer("Está regra não tem as letras que selecionou");
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                }
            }
        }
    }

    void ClearNewRuleText()
    {
        Description.text = "";
        GenericText.text = "";
        currentReplaceVariable = null;
        currentReplaceVariable2 = null;
        currentReplaceString = "";
        currentReplaceString2 = "";
    }

    void changeStep(int step)
    {
        grammar.UpdateVisibleGramar();
        grammar.SetHeaderNotSelected();
        Step = step;
        setupStep = false;
    }

    bool IsFirstStepFinished()
    {
        return grammar.IsAllHeadersWithTwoSymbols();
    }

    public void FinishTutorial()
    {
        TutorialOk = true;
        Cursor.visible = false;
        Tutorial.SetActive(false);
    }
}
