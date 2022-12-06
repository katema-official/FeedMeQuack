using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace PowerUpsNamespace
{
    public class PowerUpComponent : MonoBehaviour
    {

        [SerializeField] string _name;
        [SerializeField] string _description;
        [SerializeField] int _costDigestedbreadPoints;

        private GameObject _spriteGameobject;
        private GameObject _textsGameobject;
        private TextMesh _nameTextMesh;
        private TextMesh _descriptionTextMesh;
        private TextMesh _costTextMesh;

        private List<prova> _powerUpKinds;
        private List<float> _amountForPowerUpKind;

        private TextMesh _feedbackBought;
        private List<string> _feedbackMessages = new List<string>() { "Sold!", "Got it", "That's goooood", "Now it's mine"};



        //variable used simply to perform the destroy animation without problems
        private bool _bought = false;

        // Start is called before the first frame update
        void Start()
        {
            _powerUpKinds = new List<prova>();
            _amountForPowerUpKind = new List<float>();
            _spriteGameobject = transform.Find("Sprite").gameObject;
            _textsGameobject = transform.Find("Texts").gameObject;
            _nameTextMesh = _textsGameobject.transform.Find("Name").gameObject.GetComponent<TextMesh>();
            _descriptionTextMesh = _textsGameobject.transform.Find("Description").gameObject.GetComponent<TextMesh>();
            _costTextMesh = _textsGameobject.transform.Find("Cost").gameObject.GetComponent<TextMesh>();
            
            _feedbackBought = transform.Find("Texts/FeedbackBought").gameObject.GetComponent<TextMesh>();
            _feedbackBought.gameObject.SetActive(false);
            UpdateTexts();
        }


        private void UpdateTexts()
        {
            _nameTextMesh.text = _name;
            _descriptionTextMesh.text = _description;
            _costTextMesh.text = "Cost: " + _costDigestedbreadPoints;
        }

        // Update is called once per frame
        void Update()
        {
            //JUST FOR DEBUGGING
            /*if (Input.GetKeyDown(KeyCode.I))
            {
                Initialize("Bello", "Davvero bello", 23, null, null);
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                BuyPowerUp(23);
            }*/
        }

        public void Initialize(string name, string description, int cost, List<prova> powerUpKinds, List<float> amountForPowerUpKind)
        {
            _costDigestedbreadPoints = cost;
            _name = name;
            _description = description;
            _powerUpKinds = powerUpKinds;
            _amountForPowerUpKind = amountForPowerUpKind;
            UpdateTexts();
        }

        public enum prova
        {
            speedPU,
            aaa,
            bbb,
            ccc
        }

        public (int, List<prova>, List<float>) BuyPowerUp(int digestedBPPlayer)
        {
            if (_bought) return (0, null, null);
            //if the player doesn't have enough digested bread points, it can't buy this upgrade
            if(digestedBPPlayer < _costDigestedbreadPoints)
            {
                if (_isCantBuyCoroutineRunning == false)
                {
                    _isCantBuyCoroutineRunning = true;
                    _buyCoroutine = CantBuyCoroutine();
                    StartCoroutine(_buyCoroutine);
                }
                return (0, null, null);
            }

            //otherwise, the player can buy this powerUp. Let's tell him:
            //-how much did it spend
            //-what kind of powerUp it just bought
            //-how strong is the effect of the powerUp

            StartCoroutine(BoughtCoroutine());
            _bought = true;
            return (_costDigestedbreadPoints, _powerUpKinds, _amountForPowerUpKind);

        }







        //############################################################################################################################################################
        //#################################################################### FEEDBACK COROUTINES ###################################################################
        //############################################################################################################################################################


        private IEnumerator _buyCoroutine = null;
        bool _isCantBuyCoroutineRunning = false;
        private IEnumerator CantBuyCoroutine()
        {
            Color final = _costTextMesh.color;
            Color init = Color.red;
            float duration = 1f;
            for(float i = 0f; i < duration; i += Time.deltaTime)
            {
                Color newColor = Color.Lerp(init, final, i / duration);
                _costTextMesh.color = newColor;
                yield return null;
            }
            _costTextMesh.color = final;
            _isCantBuyCoroutineRunning = false;
            yield return null;

        }

        private IEnumerator BoughtCoroutine()
        {
            _spriteGameobject.SetActive(false);
            _nameTextMesh.gameObject.SetActive(false);
            _descriptionTextMesh.gameObject.SetActive(false);
            _costTextMesh.gameObject.SetActive(false);

            float duration = 1f;
            string textToWrite = _feedbackMessages[Random.Range(0, _feedbackMessages.Count)];
            float probabilityEasterEgg = 0.1f;
            if(Random.Range(0f, 1f) < probabilityEasterEgg)
            {
                textToWrite = "amogus";
            }
            _feedbackBought.text = textToWrite;
            _feedbackBought.gameObject.SetActive(true);
            _feedbackBought.color = Color.green;
            Color newColor = _feedbackBought.color;

            Vector3 initialPos = _feedbackBought.transform.position;
            Vector3 finalPos = new Vector3(_feedbackBought.transform.position.x, _feedbackBought.transform.position.y + 3f, _feedbackBought.transform.position.z);
            for(float i = 0f; i < duration; i+=Time.deltaTime)
            {
                newColor.a = Mathf.Lerp(1f, 0f, i / duration);
                _feedbackBought.color = newColor;

                Vector3 newPos = Vector3.Lerp(initialPos, finalPos, i / duration);
                _feedbackBought.transform.position = newPos;
                yield return null;
            }
            newColor = _feedbackBought.color;
            newColor.a = 0f;
            _feedbackBought.color = newColor;
            Destroy(this.gameObject);
            yield return null;
        }


    }
}