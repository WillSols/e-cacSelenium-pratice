using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.IO;
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");

        IWebDriver driver = new ChromeDriver(options); //Usando opções para rodar em modo headless e desativar a aceleralção gráfica

        try
        {
            string[] lines = File.ReadAllLines("config.txt");
            string accountId = "";
            string password = "";

            foreach (string line in lines)
            {
                if (line.StartsWith("AccountId:"))
                {
                    accountId = line.Replace("AccountId:", "").Trim();
                }
                else if (line.StartsWith("Password:"))
                {
                    password = line.Replace("Password:", "").Trim();
                }
            }

            driver.Navigate().GoToUrl("https://cav.receita.fazenda.gov.br/autenticacao/login");

            // Espera o botão de login do Gov.br aparecer
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(20));
            IWebElement loginButton = wait.Until(d => d.FindElement(By.CssSelector("#login-dados-certificado p input")));

            loginButton.Click();

            // Espera o campo de 'Account Id' aparecer e preenche o campo
            IWebElement accountIdField = wait.Until(d =>
            {
                var element = d.FindElement(By.Id("accountId"));
                return element.Displayed && element.Enabled ? element : null;
            });
            accountIdField.SendKeys(accountId);
            //

            IWebElement enterButton = driver.FindElement(By.Id("enter-account-id"));
            enterButton.Click();

            // Espera o campo de senha aparecer e preenche a senha
            IWebElement passwordField = wait.Until(d => d.FindElement(By.Id("password")));
            passwordField.SendKeys(password);
            //

            IWebElement submitButton = driver.FindElement(By.Id("submit-button"));
            submitButton.Click();

            // Espera que o login seja realizado com sucesso ou até que a página do e-CAC seja carregada
            wait.Until(d => d.Url.Contains("ecac"));

            Console.WriteLine("Login realizado com sucesso!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao tentar realizar login: {ex.Message}");
        }
        finally
        {
            driver.Quit();
        }
    }
}
