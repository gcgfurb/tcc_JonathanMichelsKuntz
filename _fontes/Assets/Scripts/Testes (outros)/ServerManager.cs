using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ServerManager : MonoBehaviour
{
    // Use this for initialization
    void Start()
    {
        //StartCoroutine(GetAll());
        StartCoroutine(Post());
    }

    private IEnumerator GetAll()
    {
        using (UnityWebRequest requisicao = UnityWebRequest.Get("http://dummy.restapiexample.com/api/v1/employees"))
        {
            yield return requisicao.SendWebRequest();

            if (requisicao.isNetworkError || requisicao.isHttpError)
            {
                Debug.Log(requisicao.error);
            }
            else
            {
                Debug.Log(requisicao.downloadHandler.text);
            }
        }
    }

    private IEnumerator Post()
    {
        Employee p = new Employee("Teste unity", "1000", "20");
        Debug.Log(JsonUtility.ToJson(p));
        using (UnityWebRequest requisicao = UnityWebRequest.Put("http://dummy.restapiexample.com/api/v1/create", JsonUtility.ToJson(p)))
        {
            requisicao.method = UnityWebRequest.kHttpVerbPOST;
            requisicao.SetRequestHeader("Content-Type", "application/json");
            requisicao.SetRequestHeader("Accept", "application/json");
            yield return requisicao.SendWebRequest();

            if (requisicao.isNetworkError || requisicao.isHttpError)
            {
                Debug.Log(requisicao.error);
            }
            else
            {
                Debug.Log("Upload concluido com sucesso");
                Debug.Log(requisicao.downloadHandler.text);
            }
        }
    }

    private class Employee
    {
        public string name;
        public string salary;
        public string age;

        public Employee(string nome, string salario, string idade)
        {
            name = nome;
            salary = salario;
            age = idade;
        }
    }
}
