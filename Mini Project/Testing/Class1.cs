using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mini_Project;
using NUnit.Framework;
using NUnit.Framework.Legacy;

namespace Testing
{
    [TestFixture]
    public class Class1
    {
        [Test]
        public void AdminLogin_WithValidCredentials_ReturnsTrue()
        {
            bool result = UserLogins.AdminLogin("lucky", "lucky@123");
            ClassicAssert.IsTrue(result);
        }


        [Test]
        public void AdminLogin_WithInvalidCredentials_ReturnsFalse()
        {
            bool result = UserLogins.AdminLogin("lucky", "lucky123");
            ClassicAssert.IsFalse(result);
        }

            [Test]
            public void CustomerLogin_WithValidCredentials_ReturnsTrue()
            {
                bool result = UserLogins.CustomerLogin("arla", "arla@323");

                ClassicAssert.IsTrue(result);
            }

            [Test]
            public void CustomerLogin_WithInvalidCredentials_ReturnsFalse()
            {
                bool result = UserLogins.CustomerLogin("arlaa", "arla323");

                ClassicAssert.IsFalse(result);
            }
        }
    }
