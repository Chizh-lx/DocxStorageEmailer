import React, { useState } from 'react';
import './App.css'; // Імпорт CSS файлу
import axios from 'axios';

const FormComponent = () => {
  const [email, setEmail] = useState('');
  const [file, setFile] = useState(null);
  const [emailError, setEmailError] = useState('');
  const [fileError, setFileError] = useState('');

  const handleEmailChange = (event) => {
    setEmail(event.target.value);

    if (event.target.value) {
      setEmailError('');
    }
  };

  const handleFileChange = (event) => {
    const selectedFile = event.target.files[0];
    setFile(selectedFile);

    if (event.target.value) {
      setFileError('');
    }
  };

  const handleSubmit = async (event) => {
    event.preventDefault();

    // Валідація поля електронної пошти
    if (!email) {
      setEmailError('Поле електронної пошти обов\'язкове');
    }

    // Валідація завантаженого файлу
    if (!file) {
      setFileError('Поле файлу обов\'язкове');
    }

    if (!email || !file) {
      return;
    }

    // Відправка даних на сервер або подальша обробка
    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await axios.post('https://localhost:7159/api/Values', formData, {
          headers: {
              'Content-Type': 'multipart/form-data'
          }
      });

      console.log('Відповідь від сервера:', response.data);
      alert('Файл успішно завантажено на сервер');
  } catch (error) {
      console.error('Помилка під час завантаження файлу:', error);
      alert('Сталася помилка під час завантаження файлу на сервер');
  }

  };

  return (
    <div className="form-container">
      <form onSubmit={handleSubmit} className="form">
        <div>
          <label>Електронна пошта:</label>
          <input
            type="email"
            value={email}
            onChange={handleEmailChange}
            placeholder="Введіть електронну пошту"
          />
          {emailError && <div className="error">{emailError}</div>}
        </div>
        <div>
          <label>Файл (формат .docx):</label>
          <input
            type="file"
            onChange={handleFileChange}
            accept=".docx"
          />
          {fileError && <div className="error">{fileError}</div>}
        </div>
        <button type="submit">Відправити</button>
      </form>
    </div>
  );
};

export default FormComponent;