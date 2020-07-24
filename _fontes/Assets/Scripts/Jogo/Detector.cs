using OpenCVForUnity.CoreModule;
using OpenCVForUnity.ImgcodecsModule;
using OpenCVForUnity.ImgprocModule;
using OpenCVForUnityExample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Diagnostics;
using Rect = OpenCVForUnity.CoreModule.Rect;

namespace Assets.Scripts.Jogo
{
    class Detector
    {
        private Mat MatrizImagem;
        private List<Peca> ListaPecas = null;
        private ColorBlobDetector DetectorCor;
        private readonly Scalar CorMarcacao = new Scalar(80, 30, 40); // 80,30,40
        private int IndexFrame;
        private TipoPeca TipoPecaProcessando;
        private MatOfPoint2f listaPontos;
        private List<Point> lsitaCantos;
        private double distanciaPontos;
        private List<MatOfPoint> contours;
        private List<List<Peca>> ListaFrames = null;

        public Detector()
        {
            ListaPecas = new List<Peca>();
            ListaFrames = new List<List<Peca>>();
            ListaFrames.Add(new List<Peca>()); // Frame 1
            ListaFrames.Add(new List<Peca>()); // Frame 2
            ListaFrames.Add(new List<Peca>()); // Frame 3
            this.CarregaDetectorCor();
        }

        public Mat VeriicaPecas(Mat matori, bool isCalibrando)
        {
            if (ColorBlobDetector.GetColorPoint() != null)
            {
                Mat imgMat = new Mat();
                matori.copyTo(imgMat);

                Imgproc.GaussianBlur(imgMat, imgMat, new Size(3, 3), 1, 1);

                contours = DetectorCor.GetContours();
                DetectorCor.Process(imgMat);

                if (contours.Count > 0)
                {
                    RotatedRect rect = Imgproc.minAreaRect(new MatOfPoint2f(contours[0].toArray()));

                    double boundWidth = rect.size.width;
                    double boundHeight = rect.size.height;

                    if (boundHeight > 100 && boundWidth > 100)
                    {

                        if (!isCalibrando)
                        {
                            double angleCorrecao = 0;
                            if (rect.size.width < rect.size.height)
                            {
                                angleCorrecao = 90 + rect.angle;
                            }
                            else
                            {
                                angleCorrecao = +rect.angle;
                            }

                            //Rotaciona
                            Mat matRotaciona = Imgproc.getRotationMatrix2D(new Point(matori.cols() / 2, matori.rows() / 2), angleCorrecao, 1);
                            Imgproc.warpAffine(matori, matori, matRotaciona, matori.size());
                        }

                        DetectorCor.Process(matori);
                        Rect boundRect = Imgproc.boundingRect(new MatOfPoint(contours[0].toArray()));

                        // Mostra o retangulo referente a peça principal.
                        Imgproc.rectangle(matori, boundRect.tl(), boundRect.br(), CorMarcacao, 2, 8, 0);

                        // Verifica se achou a peça base para detectar o restante
                        if (!isCalibrando)
                        {
                            boundRect = new Rect(boundRect.x, boundRect.y - boundRect.height, boundRect.width, boundRect.height);

                            // Verifica se o objeto esta dentro da imagem
                            while (
                                boundRect.x > 0 &&
                                boundRect.y > 0 &&
                                ((boundRect.x + boundRect.width) < matori.cols()) &&
                                ((boundRect.y + boundRect.height) < matori.rows()))
                            {

                                if (this.IdentificacaoPeca(new Mat(matori, boundRect), false))
                                {
                                    Imgproc.rectangle(matori, boundRect.tl(), boundRect.br(), CorMarcacao, 2, 8, 0);
                                }
                                else
                                {
                                    matori = this.MatrizImagem;
                                    break;
                                }

                                boundRect = new Rect(boundRect.x, boundRect.y - boundRect.height + 20, boundRect.width, boundRect.height);
                            }

                        }

                        if (this.IsTodosListaFramesPreenchidos())
                        {
                            this.IndexFrame = 0;
                            this.VerificaListaFrames();
                            this.LimparListaFrame();
                        }
                    }
                }
            }

            return matori;
        }

        private bool IdentificacaoPeca(Mat pecaVerifica, bool isVerificaFlechas)
        {
            this.TipoPecaProcessando = TipoPeca.INVALIDA;

            if (!isVerificaFlechas)
            {
                pecaVerifica = this.PreparaImagem(pecaVerifica);

                this.MatrizImagem = pecaVerifica;
            }

            List<MatOfPoint> contornos = new List<MatOfPoint>();
            Mat estrutura = new Mat();
            int qtdPontos = 0;
            bool isPecaValida = false;

            Imgproc.findContours(pecaVerifica, contornos, estrutura, Imgproc.RETR_LIST, Imgproc.CHAIN_APPROX_SIMPLE, new Point(0, 0)); //RETR_CCOMP

            if (contornos != null && contornos.Count > 0)
            {
    
                foreach (MatOfPoint contorno in contornos)
                {
                    MatOfPoint2f poligno = new MatOfPoint2f();
                    MatOfPoint2f curvaPolignonal = new MatOfPoint2f(contorno.toArray());
                    Imgproc.approxPolyDP(curvaPolignonal, poligno, Imgproc.arcLength(curvaPolignonal, true) * 0.02, true);
                    qtdPontos = poligno.toArray().Length;

                    if (isVerificaFlechas)
                        Debug.Log(qtdPontos);

                    if (!isVerificaFlechas && qtdPontos == 4) // Identifica o quadro
                    {
                        Rect boundRect = Imgproc.boundingRect(contorno);

                        if (Math.Abs(boundRect.width - boundRect.height) < 35)
                        {
                            //MatrizImagem = new Mat(pecaVerifica, boundRect);
                            //Imgproc.rectangle(MatrizImagem, boundRect.tl(), boundRect.br(), CorMarcacao, 2, 8, 0);

                            isPecaValida = this.IdentificacaoPeca(new Mat(pecaVerifica, boundRect), true);

                            if (isPecaValida)
                            {
                                boundRect.x = boundRect.width + boundRect.x;
                                boundRect.width = pecaVerifica.cols() - boundRect.x;
                                isPecaValida = this.VerificaQtdRepeticao(new Mat(pecaVerifica, boundRect));
                            }

                            break;
                        }
                    }
                    else if (isVerificaFlechas && (qtdPontos == 7 || qtdPontos == 10))
                    {
                        Rect boundRect = Imgproc.boundingRect(contorno);
                        boundRect.width += 1;
                        boundRect.x -= 1;

                        if (boundRect.area() > 800 && boundRect.x > 0 && boundRect.y > 0)
                        {
                            //MatrizImagem = new Mat(pecaVerifica, boundRect);

                            if (qtdPontos == 10)
                            {
                                this.TipoPecaProcessando = TipoPeca.REPETICAO;
                                Debug.Log("REPETICAO");
                            }
                            else
                            {
                                listaPontos = new MatOfPoint2f();
                                listaPontos.fromArray(poligno.toArray());
                                lsitaCantos = listaPontos.toList();
                                double[,] total = new double[lsitaCantos.Count, lsitaCantos.Count];
                                Point ponto1 = null;
                                double distanciaPontos = 0;

                                // Calcula as distancias entres os pontos
                                for (int x = 0; x < lsitaCantos.Count; x++)
                                {
                                    //Imgproc.circle(this.MatrizImagem, lsitaCantos[x], 3, CorMarcacao, 3 - 1);
                                    for (int y = 0; y < lsitaCantos.Count; y++)
                                    {
                                        if (x != y)
                                        {
                                            total[x, y] = DistanciaEuclidiana(lsitaCantos[x], lsitaCantos[y]);
                                        }
                                    }

                                }

                                // Busca pelo ponto que tem a maior soma das distancias
                                double distanciaCalc = 0;
                                int indexListaDistancias = 0;
                                for (int x = 0; x < lsitaCantos.Count; x++)
                                {
                                    distanciaCalc = SomaValores(total, x);

                                    if (distanciaCalc > distanciaPontos)
                                    {
                                        distanciaPontos = distanciaCalc;
                                        indexListaDistancias = x;
                                    }
                                }

                                ponto1 = lsitaCantos[indexListaDistancias];
                                //Imgproc.circle(this.MatrizImagem, ponto1, 3, CorMarcacao, 3 - 1);
                                this.TipoPecaProcessando = this.GetDirecao(ponto1, boundRect);
                            }

                            isPecaValida = this.TipoPecaProcessando != TipoPeca.INVALIDA;
                        }
                    }
                    else if (isVerificaFlechas)
                    {
                        listaPontos = new MatOfPoint2f();
                        listaPontos.fromArray(poligno.toArray());
                        lsitaCantos = listaPontos.toList();
                        Point ponto1 = null;

                        for (int x = 0; x < lsitaCantos.Count; x++)
                        {
                            ponto1 = lsitaCantos[x];
                            Imgproc.circle(pecaVerifica, lsitaCantos[x], 3, CorMarcacao, 3 - 1);
                        }
                    }
                }
            }

            return isPecaValida;
        }

        private bool VerificaQtdRepeticao(Mat parteVerifica)
        {
            bool isValido = false;
            if (this.TipoPecaProcessando != TipoPeca.INVALIDA)
            {
                Mat circulos = new Mat();
                Imgproc.HoughCircles(parteVerifica, circulos, Imgproc.CV_HOUGH_GRADIENT, 1, 5, 160, 10, 5, 8);
                Debug.Log(circulos.cols()); // Quantidade de pontos

                if (circulos.cols() < 13)
                {
                    this.ListaFrames[this.IndexFrame].Add(new Peca((TipoPeca)((int) this.TipoPecaProcessando), circulos.cols()));
                    isValido = true;
                }   
                else if (TipoPeca.REPETICAO == this.TipoPecaProcessando)
                {
                    this.ListaFrames[this.IndexFrame].Add(new Peca((TipoPeca)((int)this.TipoPecaProcessando), 0));
                    isValido = true;
                }

                /*
                // Mostra os pontos encontrados
                Point pt = new Point();

                for (int i = 0; i < circulos.cols(); i++)
                {
                    double[] data = circulos.get(0, i);
                    pt.x = data[0];
                    pt.y = data[1];
                    double rho = data[2];
                    Imgproc.circle(parteVerifica, pt, (int)rho, new Scalar(255, 0, 0, 255), 5);
                }
                */
            }


            return isValido;
        }

        private double SomaValores(double[,] value, int index)
        {
            double result = 0;
            for (int i = 0; i <= value.GetUpperBound(1); i++)
            {
                result += value[index, i];
            }
            return result;
        }


        private TipoPeca GetDirecao(Point ponto1, Rect seta)
        {
            bool isHorizontal = ponto1.x - 8 < 0 || ponto1.x + 8 > seta.width;

            if (isHorizontal)
            {
                if (ponto1.x - 10 < 0)
                {
                    Debug.Log("DIREITA");
                    return TipoPeca.DIREITA;
                }
                else
                {
                    Debug.Log("Esquerda");
                    return TipoPeca.ESQUERDA;
                }
            }
            else
            {
                if (ponto1.y - 10 < 0)
                {
                    Debug.Log("ABAIXO");
                    return TipoPeca.BAIXO;
                }
                else
                {
                    Debug.Log("ACIMA");
                    return TipoPeca.CIMA;
                }
            }
        }

        private Mat PreparaImagem(Mat peca)
        {
            // Utilizado mascara de 5x5 pois teve o melhor resultado. 5x5
            Imgproc.GaussianBlur(peca, peca, new Size(5, 5), 1);
            //Converte para cinza
            Imgproc.cvtColor(peca, peca, Imgproc.COLOR_RGBA2GRAY);
            Imgproc.threshold(peca, peca, 0, 255, Imgproc.THRESH_BINARY | Imgproc.THRESH_OTSU);
            // Utilizado o canny para pegar os traços da imagem
            //Imgproc.Canny(peca, peca, 60, 100, 3);
            /*
            // Fechamento da imagem
            Mat kernelMorfologico = Imgproc.getStructuringElement(Imgproc.MORPH_RECT, new Size(3, 3));
            for (int i = 0; i < 4; i++)
            {
                Imgproc.dilate(peca, peca, kernelMorfologico);
                Imgproc.erode(peca, peca, kernelMorfologico);
            }
            */
            return peca;
        }

        private double DistanciaEuclidiana(Point pontDist1, Point pontDist2)
        {
            return Math.Sqrt(Math.Pow(pontDist2.x - pontDist1.x, 2) +
                         Math.Pow(pontDist2.y - pontDist1.y, 2) * 1.0);
        }

        public void CalibrarPecas(Mat mat, GameObject fundoCalibrar)
        {
            Point pontoClicado = null;

                #if ((UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR)
                int touchCount = Input.touchCount;
                if (touchCount == 1)
                {
                    Touch t = Input.GetTouch (0);
                    if(t.phase == TouchPhase.Ended && !EventSystem.current.IsPointerOverGameObject (t.fingerId)) {
                        pontoClicado = new Point (t.position.x, t.position.y);
                    }
                }
                #else
            if (Input.GetMouseButtonUp(0))// && !EventSystem.current.IsPointerOverGameObject())
            {
                pontoClicado = new Point(Input.mousePosition.x, Input.mousePosition.y);
            }
            #endif

            if (pontoClicado != null)
            {
                ConvertScreenPointToTexturePoint(pontoClicado, pontoClicado, fundoCalibrar, mat.cols(), mat.rows());
                DetectorCor.VerificaCorPonto(mat, pontoClicado);
            }
        }

        private void CarregaDetectorCor()
        {
            DetectorCor = new ColorBlobDetector();
            float Cor1 = PlayerPrefs.GetFloat("ScalarPeca1"),
                Cor2 = PlayerPrefs.GetFloat("ScalarPeca2"),
                Cor3 = PlayerPrefs.GetFloat("ScalarPeca3"),
                Cor4 = PlayerPrefs.GetFloat("ScalarPeca4");

            if (Cor1 > 0 || Cor2 > 0 || Cor3 > 0 || Cor4 > 0)
            {
                Scalar corSalva = new Scalar(Cor1, Cor2, Cor3, Cor4);
                DetectorCor.SetColorPoint(corSalva);
                DetectorCor.SetHsvColor(corSalva);
            }
        }

        private void ConvertScreenPointToTexturePoint(Point screenPoint, Point dstPoint, GameObject textureQuad, int textureWidth = -1, int textureHeight = -1, Camera camera = null)
        {
            if (textureWidth < 0 || textureHeight < 0)
            {
                Renderer r = textureQuad.GetComponent<Renderer>();
                if (r != null && r.material != null && r.material.mainTexture != null)
                {
                    textureWidth = r.material.mainTexture.width;
                    textureHeight = r.material.mainTexture.height;
                }
                else
                {
                    textureWidth = (int)textureQuad.transform.localScale.x;
                    textureHeight = (int)textureQuad.transform.localScale.y;
                }
            }

            if (camera == null)
                camera = Camera.main;

            Vector3 quadPosition = textureQuad.transform.localPosition;
            Vector3 quadScale = textureQuad.transform.localScale;

            Vector2 tl = camera.WorldToScreenPoint(new Vector3(quadPosition.x - quadScale.x / 2, quadPosition.y + quadScale.y / 2, quadPosition.z));
            Vector2 tr = camera.WorldToScreenPoint(new Vector3(quadPosition.x + quadScale.x / 2, quadPosition.y + quadScale.y / 2, quadPosition.z));
            Vector2 br = camera.WorldToScreenPoint(new Vector3(quadPosition.x + quadScale.x / 2, quadPosition.y - quadScale.y / 2, quadPosition.z));
            Vector2 bl = camera.WorldToScreenPoint(new Vector3(quadPosition.x - quadScale.x / 2, quadPosition.y - quadScale.y / 2, quadPosition.z));

            using (Mat srcRectMat = new Mat(4, 1, CvType.CV_32FC2))
            using (Mat dstRectMat = new Mat(4, 1, CvType.CV_32FC2))
            {
                srcRectMat.put(0, 0, tl.x, tl.y, tr.x, tr.y, br.x, br.y, bl.x, bl.y);
                dstRectMat.put(0, 0, 0, 0, quadScale.x, 0, quadScale.x, quadScale.y, 0, quadScale.y);

                using (Mat perspectiveTransform = Imgproc.getPerspectiveTransform(srcRectMat, dstRectMat))
                using (MatOfPoint2f srcPointMat = new MatOfPoint2f(screenPoint))
                using (MatOfPoint2f dstPointMat = new MatOfPoint2f())
                {
                    Core.perspectiveTransform(srcPointMat, dstPointMat, perspectiveTransform);

                    dstPoint.x = dstPointMat.get(0, 0)[0] * textureWidth / quadScale.x;
                    dstPoint.y = dstPointMat.get(0, 0)[1] * textureHeight / quadScale.y;
                }
            }
        }

        private void LimparListaFrame()
        {
            if (this.ListaFrames != null)
                for (int i = 0; i < this.ListaFrames.Count; i++)
                {
                    this.ListaFrames[i].Clear();
                }
        }

        private void VerificaListaFrames()
        {
            int totalInteracao = 0, totalListaFrames = this.ListaFrames.Count;

            for (int i = 0; i < this.ListaFrames.Count; i++)
            {
                if (totalInteracao < this.ListaFrames[i].Count)
                    totalInteracao = this.ListaFrames[i].Count;
            }

            int[,] listaConferir = new int[totalListaFrames, totalInteracao];

            if (totalInteracao > 0)
            {
                for (int i = 0; i < totalInteracao; i++)
                {
                    for (int y = 0; y < totalListaFrames; y++)
                    {
                        if (this.ListaFrames[y].Count - 1 >= i)
                            for (int x = 0; x < totalListaFrames; x++)
                            {
                                if (x==y || (this.ListaFrames[x].Count - 1 >= i && 
                                    this.ListaFrames[x][i].GetTipo() == this.ListaFrames[y][i].GetTipo() && 
                                    this.ListaFrames[x][i].GetQtdRepeticao() == this.ListaFrames[y][i].GetQtdRepeticao()))
                                    listaConferir[y,i]++;
                            }
                    }
                }

                double qtdMinAcerto = Math.Round((double)listaConferir.GetLength(0) / 2, 0);

                for (int i = 0; i < totalInteracao; i++)
                {
                    for (int y = 0; y < totalListaFrames; y++)
                    {
                        if (listaConferir[y,i] >= qtdMinAcerto) 
                        {
                            this.ListaPecas.Add(this.ListaFrames[y][i]);
                            break;
                        }
                    }
                }
            }
        }

        private bool IsTodosListaFramesPreenchidos()
        {
            bool retorno = true;
            for (int i = this.IndexFrame; i < this.ListaFrames.Count; i++)
            {

                if (this.ListaFrames[i].Count == 0)
                {
                    retorno = false;
                    this.IndexFrame = i;
                    break;
                }
            }

            return retorno;
        }

        public List<Peca> GetListaPecas()
        {
            return this.ListaPecas;
        }

    }
}
