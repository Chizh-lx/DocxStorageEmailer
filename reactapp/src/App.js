import React, { useState } from 'react';
import './App.css';
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

    if (!email) {
      setEmailError('Поле електронної пошти обов\'язкове');
      return;
    }

    if (!file) {
      setFileError('Поле файлу обов\'язкове');
      return;
    }

    if (!email || !file) {
      return;
    }

    try {
      const formData = new FormData();
      formData.append('file', file);

      const response = await axios.post('https://docxuploadnotify.azurewebsites.net/api/Values', formData, {
          headers: {  
              'Content-Type': 'multipart/form-data'
          },
          params: {
            email
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
