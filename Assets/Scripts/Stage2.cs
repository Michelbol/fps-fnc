using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Stage2 : MonoBehaviour
{
    [SerializeField] Text GenericText;

    [SerializeField] Text Description;

    public bool TutorialOk = false;

    private string TutorialTitle = "Fase 2";

    public GameObject TutorialObjectTitle;

    private string TutorialRule = "- Os simbolos terminais precisam de uma regra própria, contendo apenas ele.";

    public GameObject TutorialObjectRule;

    private string TutorialHow = "- Para substituir, acerte os alvos!";

    public GameObject TutorialObjectHow;

    private string TutorialReload = "- Deu ruim? Recarregue para reiniciar (R)";

    public GameObject TutorialObjectReload;

    private AmmunationController AmmunationController;

    // Symbol not terminal(header rule) selected 
    private GameObject currentRule;
    // Index of symbol not terminal(header rule) selected
    private int currentIndex = -1;

    public GameObject AmmoCounterPannel;
    
    private GameObject currentReplaceTerminal;

    private string replaceName;

    private GameObject currentReplaceVariable;

    private AlertController alertController;

    public GameObject alert;

    public GameObject Tutorial;

    public GameObject Target;
    
    public GameObject TimerLabel;

    public Grammar grammar;

    private int Step = 0;

    private int selectedHeader;

    private Utils utils = new Utils();

    private bool setupStep = false;

    // Start is called before the first frame update
    void Start()
    {
        TutorialObjectTitle.GetComponent<Text>().text = TutorialTitle;
        TutorialObjectRule.GetComponent<Text>().text = TutorialRule;
        TutorialObjectHow.GetComponent<Text>().text = TutorialHow;
        TutorialObjectReload.GetComponent<Text>().text = TutorialReload;

        alertController = alert.GetComponent<AlertController>();
        AmmunationController = AmmoCounterPannel.GetComponent<AmmunationController>();
        AmmunationController.GameEnable = false;
        grammar.ParseGramar();
        grammar.UpdateVisibleGramar();
    }

    void resetGame()
    {
        grammar.ParseGramar();
        Target.SetActive(false);
        AmmunationController.GameEnable = false;
    }

    // Update is called once per frame
    void Update()
    {
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
            activeObjects[0] = Target;

            GameObject[] inactiveObjects = new GameObject[0];

            if (utils.startTimer(activeObjects, inactiveObjects, TimerLabel, 3))
            {
                return;
            }
            Target.SetActive(true);
            AmmunationController.GameEnable = true;
            changeStep(1);
        }
        if (Step == 1)
        {
            if (!setupStep)
            {
                grammar.SetHeaderRuleActive();
                Description.text = "Atire em uma Letra Maiúscula, que tenha letras mínusculas";
                setupStep = true;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                if (!grammar.selectHeaderHasOneTerminal(indexHeader))
                {
                    //AVISO: Gramática não pode ser substituida
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                    alertController.startTimer("Essa regra não tem uma letra minúscula!");
                    return;
                }
                selectedHeader = indexHeader;
                grammar.SetHeaderRuleInactive();
                grammar.SetHeaderNotSelected();
                grammar.SetHeaderActive();
                changeStep(2);
            }
        }
        if (Step == 2)
        {
            if (!setupStep)
            {
                currentRule = grammar.SetRuleActive(selectedHeader);
                currentIndex = selectedHeader;
                Description.text = "Atire em uma letra minúscula para substituirmos";
                setupStep = true;
            }
            List<GameObject> selectedObjects = grammar.IsSomeRuleSelected(selectedHeader);
            if (selectedObjects.Count > 0)
            {
                if (!char.IsLower(selectedObjects[0].name[0]))
                {
                    alertController.startTimer("Essa letra não é minúscula!");
                    grammar.SetTargetRulesActive(selectedHeader);
                    grammar.SetRuleNotSelected(selectedHeader);
                    grammar.SetTargetRulesActiveAndNotSelectedAndFixPosition(selectedHeader);
                    Description.text = "";
                    grammar.SetCurrentRuleInactive(currentIndex);
                    changeStep(1);
                    return;
                }
                if (grammar.findRuleWithOnlyTerminal(selectedObjects[0].name))
                {
                    //Aviso: Esses simbolos não podem ser substituidos
                    alertController.startTimer("Esses simbolos já tem uma variável terminal");
                    grammar.SetTargetRulesActive(selectedHeader);
                    grammar.SetRuleNotSelected(selectedHeader);
                    grammar.SetTargetRulesActiveAndNotSelectedAndFixPosition(selectedHeader);
                    Description.text = "";
                    grammar.SetCurrentRuleInactive(currentIndex);
                    changeStep(1);
                    return;
                }
                currentReplaceTerminal = selectedObjects[0];
                changeStep(3);
                grammar.SetCurrentRuleInactive(currentIndex);
            }
        }
        if (Step == 3)
        {
            if (IsFirstStepFinished())
            {
                alertController.successMessage("Parabéns!! Você conseguiu finalizar a segunda etapa!");
                if (utils.setTimeOut(3))
                {
                    return;
                }
                Cursor.visible = true;
                SceneManager.LoadScene("SuccessStage2");
                return;
            }
            if (!setupStep)
            {
                replaceName = currentReplaceTerminal.name;
                Description.text = "Atire nas regras que possuem a letra que atirou.";
                GenericText.text = $"Letra: {replaceName}";
                currentReplaceVariable = grammar.replaceTerminalRuleByDefaultRule(currentReplaceTerminal, currentIndex);
                grammar.createNewHeaderWithOneRule(currentReplaceVariable.name, replaceName);
                grammar.FixPosition(selectedHeader);
                grammar.UpdateVisibleGramar();
                grammar.SetHeaderRuleActive();
                setupStep = true;
            }
            if (grammar.findRuleWithOnlyTerminal(replaceName) && !grammar.HasMoreThanTwoRulesHavingTerminal(replaceName))
            {
                alertController.startTimer("Não tem mais nenhuma regra para selecionar, faça isso para a próxima");
                grammar.SetHeaderRuleInactive();
                grammar.SetHeaderNotSelected();
                grammar.SetHeaderActive();
                changeStep(1);
                return;
            }
            int indexHeader = grammar.IsSomeHeaderSelected();
            if (indexHeader != -1)
            {
                if (grammar.AreHeadersEquals(currentReplaceVariable.name, indexHeader))
                {
                    alertController.startTimer("Está regra é a nova regra, escolha outra");
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                    return;
                }
                var obj = grammar.FindSymbolRulesByHeaderAndName(indexHeader, replaceName);
                if(obj == null)
                {
                    alertController.startTimer("Está regra não tem a letra selecionada");
                    grammar.SetHeaderNotSelected();
                    grammar.SetHeaderActive();
                    return;
                }
                grammar.replaceTerminalRuleByVariable(obj, currentReplaceVariable.name, indexHeader);
                grammar.UpdateVisibleGramar();
                grammar.SetHeaderRuleActive();
                grammar.FixPosition(selectedHeader);
                grammar.FixPositionHeaders();
            }
        }
    }

    void changeStep(int step)
    {
        GenericText.text = "";
        Description.text = "";
        grammar.UpdateVisibleGramar();
        grammar.SetHeaderNotSelected();
        grammar.FixPositionHeaders();
        Step = step;
        setupStep = false;
    }

    public void FinishTutorial()
    {
        TutorialOk = true;
        Cursor.visible = false;
        Tutorial.SetActive(false);
    }

    bool IsFirstStepFinished()
    {
        return grammar.IsAllTerminalsSeparatedAndSingle();
    }
}

