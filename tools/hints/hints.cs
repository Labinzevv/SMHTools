using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways] // скрипт работает в редакторе
public class hints : MonoBehaviour
{
    [Space(10, order = 0)] // расстояние до заголовка
    [Header("name")] // "имя над полем"
    [Space(40, order = 2)] // расстояние после заголовка
    string str;
    Text text = null;
    bool select;
    char ch;
    int index = 0;
    double indexDouble = 0;
    float f;
    GameObject[] objMassive = null;
    GameObject[] objMassive2 = null;
    GameObject obj = null;
    List<GameObject> listObjMassive = null;
    List<int> listIntMassive = null;
    int[] intMassive;
    Image image = null;
    InputField InpField;
    Animator anim;

    // метод срабатывет в редакторе, после внесения изменений в инспектор скрипта
    void OnValidate()
    {

    }

    void Update()
    {
        //последний символ строки str присваивает "1"(можно указать любой символ) (использовать в if)
        str.EndsWith("1");
      
        //строка str принимает значение Text text
        str = text.text;

        //делит indexDouble / index (результат в indexDouble)
        indexDouble = indexDouble / index;
        //округляет результат деления в ближнюю сторону (если результат 1.4 округляет до 1, если результат 1.6 округляет до 2)
        indexDouble = Math.Round(indexDouble);

        /* округляет число indexDouble до кол-ва знаков после запятой указанных в скобках 
           (1 указатель кол-ва знаков после запятой) (если indexDouble == 1.3456 то округлится до 1.3) */
        indexDouble = Math.Round(indexDouble, 1);

        //округляет число в большую сторону (если indexDouble == 1.3456 то округлится до 2)
        indexDouble = Math.Ceiling(indexDouble);

        //округляет число в меньшую сторону (если indexDouble == 1.3456 то округлится до 1)
        indexDouble = Math.Floor(indexDouble);

        //float в string и количество знаков после запятой
        f.ToString("0.0");

        //шаблон if
        if (str != "") //если строка str не пуста
        {
            //добавить действие
        }

        //проверка проигрывается ли анимация "name"
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("name"))
        {
            //добавить действие
        }

        //отнимает один (последний) символ от Text text
        text.text = text.text.Remove(text.text.Length - 1);

        //Text text обнуляется (становится пустым)
        text.text = "";

        //в  Text text добавляется символ "1" (в конец поля)
        text.text = text.text + "1";

        //символ ch обнуляется (становится пустым)
        ch = '\0';    
        
        //символ ch присваивает "A" (можно указать любой символ)
        ch = 'A';

        //символ ch присваивает значение первого символа строки str
        ch = str[0];

        //bool select при каждом выполнении меняет своё значение на противоположное(использовать как переключатель)     
        select = !select;

        //удаляет всё содержимое массива objMassive
        for (int i = 0; i < objMassive.Length; i++)
        {
            Destroy(objMassive[i]);
        }

        //удаляет последний элемент массива objMassive
        Destroy(objMassive[objMassive.Length - 1]);
      
        //в массив objMassive добавляются найденные объекты с тегом "tag"
        objMassive = GameObject.FindGameObjectsWithTag("tag");

        //копирует objMassive в objMassive2
        objMassive2 = (GameObject[])objMassive.Clone();

        //спавнит объекты которые находятся в массиве objMassive
        for (int i = 0; i <= objMassive.Length; i++)
        {
            /* index добавляется каждый цикл и кол во спавнов будут равны длине массива objMassive
               или указать определённое число index = 10; (10 спавнов) */
            index++;
            if (index < objMassive.Length)
            {
                //ищет объекты по тегу "tag" и добавляет их в массив objMassive
                objMassive = GameObject.FindGameObjectsWithTag("tag");
                //спавн объектов которые находятся в массиве objMassive
                GameObject spawn = Instantiate(objMassive[i]);
                spawn.name = "New Name"; //указать имя заспавненным объектам
                spawn.name = spawn.name.Replace("(Clone)", ""); //убрать "(Clone)" заспавненным объектам
            }       
        }

        //obj принимает значение найденного объекта с именем "name"
        obj = GameObject.Find("name");

        //Спавн объекта наследованного от префаба
        GameObject objSpawn = Instantiate(obj);
        //Удаление заспавненного объекта
        Destroy(objSpawn);

        //удаляет элемент массива objMassive с индексом присвоенным из int index
        Destroy(objMassive[index]);

        //шаблон цикла for
        for (int i = 0; i < objMassive.Length; i++)
        {
            //сюда поместить действие
        }

        // шаблон цикла foreach
        foreach (int intSearch in intMassive)
        {
            //сюда поместить действие
        }

        /* переменная int index принимает значение InputField InpField(значение InpField должно быть целочисленным)
           (так-же вместо int может быть string или float) */
        index = int.Parse(InpField.text);
        //сохраняет значение переменной index в ключ "index" в реестре (имя ключа может быть любым)
        PlayerPrefs.SetInt("index", index);
        //возвращает (переменная index принимает) значение ключа "index"
        index = PlayerPrefs.GetInt("index");
        /* (КРАТКОЕ ОПИСАНИЕ СОХРАНЕНИЯ ДАННЫХ С ПОМОЩЬЮ МЕТОДА PlayerPrefs) переменная index принимает значение InpField.text, 
           далее в ключе "index" сохраняется значение переменной index, далее переменная index принимает значение ключа "index" */

        //корутина wait
        StartCoroutine(wait());
        IEnumerator wait()
        {
            yield return new WaitForSeconds(0.1f);
            //сюда поместить действие
        }

        //в List listObjMassive добавляется найденный объект с тегом "tag"
        listObjMassive.Add(GameObject.FindGameObjectWithTag("tag"));
       
        //в List listObjMassive добавляются найденные объекты с тегом "tag"
        listObjMassive.AddRange(GameObject.FindGameObjectsWithTag("tag"));

        //удаляет последний элемент listObjMassive (удаляет даже самый первый элемент)
        listObjMassive.RemoveAt(listObjMassive.Count - 1);

        //удаляет последний элемент listObjMassive (удаляет до первого элемента)
        listObjMassive.Remove(listObjMassive.Count - 1);

        //добавляет в List listIntMassive число 1
        listIntMassive.Add(1);

        //очищает List listIntMassive
        listIntMassive.Clear();

        //список listIntMassive заполняется своим-же содержимым, пока не станет равен index
        for (int i = 0; i < listIntMassive.Count; i++)
        {
            if (index > listIntMassive.Count)
            {
                listIntMassive.AddRange(listIntMassive);
            }
        }

        //int index берёт значение количества элементов "1" в списке listIntMassive
        index = listIntMassive.Count(ei => ei == 1);

        /* если int index не равен количеству элементов "1" в списке listIntMassive то index = 0
           (то есть если в listIntMassive нет ни одного элемента "1" то index = 0) */
        if (index != listIntMassive.Count(ei => ei == 1))
        {
            index = 0;
        }

        //если длинна списка = 0 (то есть список пуст, то index = 0)
        if (listIntMassive.Count == 0)
        {
            index = 0;
        }

        //плавно изменяется цвет Image image
        image.color = Color.Lerp(Color.white, new Color(1, 0.8f, 0f), Mathf.Sin(Time.time * 12));

        //указывает конкретный цвет для Image image eng
        image.color = Color.white;

        /* функция проверки "есть ли запись, находящаяся в переменной string str в списке listIntMassive"
           если есть, переменная Text text принимает "Это слово есть в listIntMassive",
           к int index добавляется ++ (1), если нет содержимое
           string str добавляется в listIntMassive, Text text принимает "", а int index = 0 */
        foreach (string a in listIntMassive)
        {
            if (a.Contains(str))
            {
                if (str.Length > 0)
                {
                    text.text = "Это слово есть в listIntMassive";
                    str = "";
                }
            }
            if (str == a)
            {
                index++;
            }
            if (str != a)
            {
                index = 0;
            }
        }
        if (index == 0)
        {
            if (str.Length > 0)
            {
                listIntMassive.Add(str);
                text.text = "";
                index = 0;
            }
        }
    }

    //метод регистрирует клик по объекту (на объекте должен быть коллайдер)
    void OnMouseDown()
    {
        //здесь должно быть действие, выполняющееся при клике по объекту
    }
}
