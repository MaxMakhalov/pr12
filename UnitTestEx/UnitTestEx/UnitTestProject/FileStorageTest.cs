using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Reflection;
using UnitTestEx;
using Assert = NUnit.Framework.Assert;

namespace UnitTestProject
{
    /// <summary>
    /// Summary description for FileStorageTest
    /// </summary>
    [TestClass]
    public class FileStorageTest
    {
        public const string MAX_SIZE_EXCEPTION = "DIFFERENT MAX SIZE";
        public const string NULL_FILE_EXCEPTION = "NULL FILE";
        public const string NO_EXPECTED_EXCEPTION_EXCEPTION = "There is no expected exception";

        public const string SPACE_STRING = " ";
        public const string FILE_PATH_STRING = "@D:\\JDK-intellij-downloader-info.txt";
        public const string CONTENT_STRING = "Some text";
        public const string REPEATED_STRING = "AA";
        public const string WRONG_SIZE_CONTENT_STRING = "TEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtextTEXTtext";
        public const string TIC_TOC_TOE_STRING = "tictoctoe.game";

        public const int NEW_SIZE = 5;

        public FileStorage storage = new FileStorage(NEW_SIZE);

        /* ПРОВАЙДЕРЫ */

        static object[] NewFilesData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) },
            new object[] { new File(SPACE_STRING, CONTENT_STRING) },
            new object[] { new File(FILE_PATH_STRING, CONTENT_STRING) }
        };

        static object[] FilesForDeleteData =
        {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING), REPEATED_STRING },
            new object[] { new File(TIC_TOC_TOE_STRING, CONTENT_STRING), TIC_TOC_TOE_STRING }
        };

        static object[] NewExceptionFileData = {
            new object[] { new File(REPEATED_STRING, CONTENT_STRING) }
        };


		/* Тестирование записи файла */
		[Test, TestCaseSource(nameof(NewFilesData))]
        public void WriteTest(File file) 
        {
			storage.Delete("AA");
			String name = file.GetFilename();
			Assert.True(storage.Write(file), $"Cannot write {name}");
			storage.DeleteAllFiles();
		}

        /* Тестирование записи дублирующегося файла */
        [Test, TestCaseSource(nameof(NewExceptionFileData))]
        public void WriteExceptionTest(File file) {
            bool isException = false;
            try
            {
                storage.Write(file);
                storage.Write(file);
			}
			catch (FileNameAlreadyExistsException)
            {
                isException = true;
			}
            finally
            {
                storage.DeleteAllFiles();
            }
			Assert.True(isException, NO_EXPECTED_EXCEPTION_EXCEPTION);
        }

        /* Тестирование проверки существования файла *///
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void IsExistsTest(File file) {
            storage.Write(file);
			String name = file.GetFilename();
			Assert.True(storage.IsExists(name), $"File {name} doesn't exists");
			storage.DeleteAllFiles();
		}

        /* Тестирование удаления файла */
        [Test, TestCaseSource(nameof(FilesForDeleteData))]
        public void DeleteTest(File file, String fileName) {
            storage.Write(file);
            Assert.True(storage.Delete(fileName), $"Cannot delete {fileName}.");
		}

        /* Тестирование получения файлов */
        [Test]
        public void GetFilesTest()
        {
            foreach (File el in storage.GetFiles()) 
            {
                Assert.NotNull(el);
            }
        }

        // Почти эталонный
        /* Тестирование получения файла *///
        [Test, TestCaseSource(nameof(NewFilesData))]
        public void GetFileTest(File expectedFile) 
        {
			storage.Write(expectedFile);
			bool isException = false;
			try
			{
				File actualfile = storage.GetFile(expectedFile.GetFilename());
			}
			catch (Exception)
			{
				isException = true;
			}
            finally
            {
				storage.DeleteAllFiles();
			}
			Assert.True(!isException, "GetFileFailed");
		}
        [Test]
        public void GetFile_NonExistentFile_ReturnsNull() //Проверка поведения GetFile при отсутствии файла
        {
            // Arrange
            var storage = new FileStorage();
            var fileName = "ghost.txt";

            // Act
            var result = storage.GetFile(fileName);

            // Assert
            Assert.IsNull(result, "GetFile должен возвращать null для несуществующих файлов");
        }
        [Test]
        public void GetFiles_MultipleFiles_ReturnsAllWrittenFiles() //Проверка получения всех файлов после нескольких записей
        {
            // Arrange
            var storage = new FileStorage();
            var file1 = new File("file1.txt", "abc");
            var file2 = new File("file2.txt", "defg");
            storage.Write(file1);
            storage.Write(file2);

            // Act
            var files = storage.GetFiles();

            // Assert
            Assert.Contains(file1, files, "В списке файлов отсутствует file1");
            Assert.Contains(file2, files, "В списке файлов отсутствует file2");
            Assert.AreEqual(2, files.Count, "Количество файлов в хранилище должно быть 2");
        }

    }
}
