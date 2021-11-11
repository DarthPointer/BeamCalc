using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeamCalc.Project;

namespace BeamCalc.Solver
{
    class Solution
    {
        public static SolutionResultData Solve(ProjectData project, string relativePath)
        {
            if (project.valid)
            {
                SolutionResultData.SolutionBeam[] solutionBeams = project.beams.Select(x => new SolutionResultData.SolutionBeam(x.Value, x.Key, project)).
                    OrderBy(x => project.nodes[x.leftNode].location).ToArray();

                Matrix offsetSolutionMatrix = new Matrix(solutionBeams.Length + 1, solutionBeams.Length + 2);

                if (project.nodes[solutionBeams[0].leftNode].xFixed)
                {
                    offsetSolutionMatrix.AddAt(0, 0, 1);
                }

                for (int i = 0; i < solutionBeams.Length; i++)
                {
                    SolutionResultData.SolutionBeam beam = solutionBeams[i];

                    double matrixCoeff = project.materialDataStorage.materials[beam.materialName].elasticModulus * beam.crossSection / beam.length;

                    if (!project.nodes[beam.leftNode].xFixed)
                    {
                        offsetSolutionMatrix.AddAt(i, i, matrixCoeff);
                    }

                    if (project.nodes[beam.rightNode].xFixed)
                    {
                        offsetSolutionMatrix.AddAt(i + 1, i + 1, 1);
                    }
                    else
                    {
                        offsetSolutionMatrix.AddAt(i + 1, i + 1, matrixCoeff);

                        if (!project.nodes[beam.leftNode].xFixed)
                        {
                            offsetSolutionMatrix.AddAt(i + 1, i, -matrixCoeff);
                            offsetSolutionMatrix.AddAt(i, i + 1, -matrixCoeff);
                        }
                    }
                }

                if (!project.nodes[solutionBeams[0].leftNode].xFixed)
                {
                    offsetSolutionMatrix.AddAtLastColumn(0, project.nodes[solutionBeams[0].leftNode].xForce);
                }

                for (int i = 0; i < solutionBeams.Length; i++)
                {
                    SolutionResultData.SolutionBeam beam = solutionBeams[i];

                    double cumulativeReaction = beam.length * beam.AbsoluteLoad / 2;

                    if (!project.nodes[beam.leftNode].xFixed)
                    {
                        offsetSolutionMatrix.AddAtLastColumn(i, cumulativeReaction);
                    }

                    if (!project.nodes[beam.rightNode].xFixed)
                    {
                        offsetSolutionMatrix.AddAtLastColumn(i + 1, cumulativeReaction);
                        offsetSolutionMatrix.AddAtLastColumn(i + 1, project.nodes[beam.rightNode].xForce);
                    }
                }

                double[] solution = offsetSolutionMatrix.GaussSolve();

                IEnumerator<double> solutionEnumerator = solution.AsEnumerable().GetEnumerator();
                solutionEnumerator.MoveNext();

                foreach (SolutionResultData.SolutionBeam solutionBeam in solutionBeams)
                {
                    double E = project.materialDataStorage.materials[solutionBeam.materialName].elasticModulus;
                    double A = solutionBeam.crossSection;
                    double L = solutionBeam.length;

                    double u0 = solutionEnumerator.Current;
                    solutionEnumerator.MoveNext();
                    double ul = solutionEnumerator.Current;

                    double q = solutionBeam.AbsoluteLoad;

                    solutionBeam.reaction = new SolutionResultData.SqareFunction()
                    {
                        a2 = 0,
                        a1 = -q,
                        a0 = (E * A / L) * (ul - u0) + (q * L / 2)
                    };

                    solutionBeam.offset = new SolutionResultData.SqareFunction()
                    {
                        a2 = - q / (2 * E * A),
                        a1 = - u0 / L + ul / L + (q * L) / (2 * E * A),
                        a0 = u0
                    };
                }

                SolutionResultData result = new SolutionResultData()
                {
                    nodes = project.nodes,
                    materials = project.materialDataStorage.materials,

                    beams = solutionBeams,

                    filePath = project.folder + relativePath
                };

                return result;
            }
            else
            {
                throw new Exception("Project is not valid");
            }
        }


        class Matrix
        {
            double[][] body;
            int m;
            int n;

            public Matrix(int m, int n)
            {
                this.m = m;
                this.n = n;

                body = new double[m][];

                for (int i = 0; i < m; i++)
                {
                    body[i] = new double[n];
                }
            }

            public void AddAt(int row, int column, double value)
            {
                body[row][column] += value;
            }

            public void AddAtLastColumn(int row, double value)
            {
                AddAt(row, n - 1, value);
            }

            public override string ToString()
            {
                return string.Join('\n', body.Select(x => string.Join(' ', x.Select(StringLib.DisplayedString))));
            }

            public double[] GaussSolve()
            {
                for (int i = 0; i < m - 1; i++)
                {
                    Eliminate(i, i + 1);
                }

                for (int i = m - 1; i > 0; i--)
                {
                    Eliminate(i, i - 1);
                }

                double[] result = new double[m];

                for (int i = 0; i < m; i++)
                {
                    result[i] = body[i][n - 1] / body[i][i];
                }

                return result;
            }

            void Eliminate(int argIndex, int equationIndex)
            {
                double eliminator = body[argIndex][argIndex];
                double target = body[equationIndex][argIndex];

                ApplyEquation(equationIndex, argIndex, -target / eliminator);
            }

            void ApplyEquation(int targetEquationIndex, int sourceEquationIndex, double ratio)
            {
                for (int i = 0; i < n; i++)
                {
                    body[targetEquationIndex][i] += body[sourceEquationIndex][i] * ratio;
                }
            }
        }
    }
}
