import { environment } from 'src/environments/environment';

export const CKEditorFullConfiguration = {
  toolbar: {
    items: [
      'undo',
      'redo',
      '|',
      'heading',
      'fontSize',
      'fontFamily',
      '|',
      'bold',
      'italic',
      'underline',
      'fontColor',
      '|',
      'bulletedList',
      'numberedList',
      '|',
      'alignment',
      'outdent',
      'indent',
      '|',
      'insertTable',
      'imageInsert',
      'mediaEmbed',
      '|',
      'blockQuote',
      'link',


    ],
  },
  language: 'en',
  image: {
    resizeOptions: [
      {
        name: 'resizeImage:original',
        value: null,
        icon: 'original',
      },
      {
        name: 'resizeImage:50',
        value: '50',
        icon: 'medium',
      },
      {
        name: 'resizeImage:75',
        value: '75',
        icon: 'large',
      },
    ],
    toolbar: [
      'imageTextAlternative',
      'toggleImageCaption',
      'imageStyle:inline',
      'imageStyle:block',
      'imageStyle:side',
      'resizeImage:50',
      'resizeImage:75',
      'resizeImage:original',
    ],
  },
  table: {
    contentToolbar: ['tableColumn', 'tableRow', 'mergeTableCells'],
  },
};

export class CkEditorUploadAdapter {
  xhr = new XMLHttpRequest();
  jwtToken: string;
  baseUrl = environment.apiUrl;
  loader: any;

  constructor(private fileLoader: any, private token: any) {
    this.loader = fileLoader;
    this.jwtToken = token;
  }

  upload() {
    return this.loader.file.then(
      (file) =>
        new Promise((resolve, reject) => {
          this._initRequest();
          this._initListeners(resolve, reject, file);
          this._sendRequest(file);
        })
    );
  }
  abort() {
    if (this.xhr) {
      this.xhr.abort();
    }
  }
  _initRequest() {
    const xhr = (this.xhr = new XMLHttpRequest());
    xhr.open('POST', this.baseUrl + 'file/image?width=0&height=0', true);
    xhr.responseType = 'json';
    xhr.setRequestHeader('Accept-Language', 'vi,en-US;q=0.9,en;q=0.8');
    xhr.setRequestHeader('Accept', '*/*');
    xhr.setRequestHeader('scheme', 'https');
    xhr.setRequestHeader('Authorization', 'Bearer ' + this.jwtToken);
  }
  _initListeners(resolve, reject, file) {
    const xhr = this.xhr;
    const loader = this.loader;
    const genericErrorText = `Couldn't upload file: ${file.name}.`;
    xhr.addEventListener('error', () => reject(genericErrorText));
    xhr.addEventListener('abort', () => reject());
    xhr.addEventListener('load', () => {
      const response = xhr.response;
      if (!response || response.error) {
        return reject(
          response && response.error ? response.error.message : genericErrorText
        );
      }
      resolve({
        default: response.url,
      });
    });
    if (xhr.upload) {
      xhr.upload.addEventListener('progress', (evt) => {
        if (evt.lengthComputable) {
          loader.uploadTotal = evt.total;
          loader.uploaded = evt.loaded;
        }
      });
    }
  }
  _sendRequest(file) {
    const data = new FormData();
    data.append('file', file);
    this.xhr.send(data);
  }
}
