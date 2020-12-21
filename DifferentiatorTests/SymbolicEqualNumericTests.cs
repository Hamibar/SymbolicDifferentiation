using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq.Expressions;

namespace SymbolicDifferentiation
{
    [TestClass]
    public class SymbolicEqualNumericTests
    {
        void AssertSyntacticDerivativeEqualToNumericDerivative(Expression<Func<double, double>> func)
        {
            var f = func.Compile();
            double eps = 1e-7;
            var dfExpression = Differentiator.Differentiate(func);
            var df = dfExpression.Compile();
            for (double x = 0.5; x < 10; x += 0.5)
            {
                Assert.AreEqual((f(x + eps) - f(x - eps)) / (2 * eps), df(x), 1e-4);
            }
        }

        [TestMethod]
        public void DifferentiateConstant()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => 1);
        }

        [TestMethod]
        public void DifferentiateParameter()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => z);
        }

        [TestMethod]
        public void DifferentiateLinearFunction()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => 10 * z);
        }

        [TestMethod]
        public void DifferentiateQuadraticFunction()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => 10 * z * z);
        }

        [TestMethod]
        public void DifferentiateDivide1()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => z / 10);
        }

        public void DifferentiateDivide2()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => z / (10 * z));
        }

        [TestMethod]
        public void DifferentiateSum()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => z + z);
        }

        [TestMethod]
        public void DifferentiateSubstract()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => z - z);
        }

        [TestMethod]
        public void DifferentiateSumAndProduct()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => 10 * z + z * z);
        }

        [TestMethod]
        public void DifferentiateSin1()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Sin(z));
        }

        [TestMethod]
        public void DifferentiateSin2()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Sin(z * z + z));
        }

        [TestMethod]
        public void DifferentiateCos1()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(z));
        }

        [TestMethod]
        public void DifferentiateCos2()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(z * z + z));
        }

        [TestMethod]
        public void DifferentiateComplexExpression1()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(Math.Sin(z) * Math.Cos(z)));
        }

        [TestMethod]
        public void DifferentiateComplexExpression2()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(Math.Sin(z) * Math.Sin(z) * 10));
        }

        [TestMethod]
        public void DifferentiateComplexExpression3()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(10 * z * z + Math.Cos(z * z * Math.Sin(z))));
        }

        [TestMethod]
        public void DifferentiateComplexExpression4()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(z) / Math.Sin(z));
        }

        [TestMethod]
        public void DifferentiateComplexExpression5()
        {
            AssertSyntacticDerivativeEqualToNumericDerivative(z => Math.Cos(Math.Sin(z * z * 10)) / Math.Sin(Math.Cos(z / Math.Sin(z))));
        }
    }
}
