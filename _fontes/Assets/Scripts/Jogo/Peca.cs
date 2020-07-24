using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Jogo
{
    class Peca
    {
        private TipoPeca Tipo = TipoPeca.INVALIDA;
        private int QtdRepeticao = 0;
        private readonly string CaminhoBaseImgs = "Prefabs/InterfaceTangivel/";

        public Peca(TipoPeca Tipo, int QtdRepeticao)
        {
            this.SetTipo(Tipo);
            this.SetQtdRepeticao(QtdRepeticao);
        }

        public TipoPeca GetTipo()
        {
            return this.Tipo;
        }

        public void SetTipo(TipoPeca peca)
        {
            this.Tipo = peca;
        }

        public int GetQtdRepeticao()
        {
            return this.QtdRepeticao;
        }

        public void SetQtdRepeticao(int qtd)
        {
            this.QtdRepeticao = qtd;
        }

        public Sprite GetDirecaoImagem()
        {
            switch (this.Tipo)
            {
                case TipoPeca.BAIXO:
                    return Resources.Load(CaminhoBaseImgs + "Direcao_baixo", typeof(Sprite)) as Sprite;
                case TipoPeca.CIMA:
                    return Resources.Load(CaminhoBaseImgs + "Direcao_cima", typeof(Sprite)) as Sprite;
                case TipoPeca.ESQUERDA:
                    return Resources.Load(CaminhoBaseImgs + "Direcao_esquerda", typeof(Sprite)) as Sprite;
                case TipoPeca.DIREITA:
                    return Resources.Load(CaminhoBaseImgs + "Direcao_direita", typeof(Sprite)) as Sprite;
                case TipoPeca.REPETICAO:
                    return Resources.Load(CaminhoBaseImgs + "Repeticao", typeof(Sprite)) as Sprite;
            }

            return null;
        }

        public Sprite GetQuantidadeImagem()
        {
            if (this.Tipo != TipoPeca.INVALIDA)
            {
                switch (this.QtdRepeticao)
                {
                    case 1:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade1", typeof(Sprite)) as Sprite;
                    case 2:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade2", typeof(Sprite)) as Sprite;
                    case 3:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade3", typeof(Sprite)) as Sprite;
                    case 4:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade4", typeof(Sprite)) as Sprite;
                    case 5:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade5", typeof(Sprite)) as Sprite;
                    case 6:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade6", typeof(Sprite)) as Sprite;
                    case 7:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade7", typeof(Sprite)) as Sprite;
                    case 8:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade8", typeof(Sprite)) as Sprite;
                    case 9:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade9", typeof(Sprite)) as Sprite;
                    case 10:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade10", typeof(Sprite)) as Sprite;
                    case 11:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade11", typeof(Sprite)) as Sprite;
                    case 12:
                        return Resources.Load(this.CaminhoBaseImgs + "Quantidade12", typeof(Sprite)) as Sprite;
                }
            }

            return null;
        }

        public Sprite GetTipoImagem()
        {
            if (this.GetTipo() != TipoPeca.PLAY)
            {
                return Resources.Load(CaminhoBaseImgs + "Pecas", typeof(Sprite)) as Sprite;
            }
            else
            {
                return Resources.Load(CaminhoBaseImgs + "Play", typeof(Sprite)) as Sprite;
            }
        }
    

        public string GetComando(string comandos)
        {
            string retorno = "";

            if (this.Tipo != TipoPeca.INVALIDA)
            {
                string comando = "";
                
                switch (this.Tipo)
                {
                    case TipoPeca.BAIXO:
                        comando = "andar(abaixo);";
                        break;
                    case TipoPeca.CIMA:
                        comando = "andar(acima);";
                        break;
                    case TipoPeca.ESQUERDA:
                        comando = "andar(esquerda);";
                        break;
                    case TipoPeca.DIREITA:
                        comando = "andar(direita);";
                        break;
                    case TipoPeca.REPETICAO:
                        if (this.QtdRepeticao == 1)
                            comando = "";
                        else
                            comando = comandos;
                        break;
                }

                if (this.QtdRepeticao>1)
                {
                    for (int ind = (this.GetTipo() == TipoPeca.REPETICAO ? 1 : 0); ind < this.QtdRepeticao; ind++)
                    {
                        retorno += (retorno.Length==0 ? "" : "\n") + comando;
                    }
                }
                else
                {
                    retorno = comando;
                }
            }

            return retorno;
        }
    }
}
