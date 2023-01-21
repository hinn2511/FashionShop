import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import * as CKEditor from 'src/build/ckeditor';
import {
  CKEditorFullConfiguration,
  CkEditorUploadAdapter,
} from 'src/app/_forms/editor/ckeditor';

@Component({
  selector: 'app-editor',
  templateUrl: './editor.component.html',
  styleUrls: ['./editor.component.css'],
})
export class EditorComponent implements OnInit {
  @Input() CKConfig = CKEditorFullConfiguration;
  @Input() initialContent: string = '';
  @Output() outputContent = new EventEmitter<string>();
  content: string;
  public CKEditor = CKEditor;
  constructor(private authenticationService: AuthenticationService) {
    //Not implement
  }

  ngOnInit(): void { 
    this.content = this.initialContent;
  }

  contentChange() {
    this.outputContent.emit(this.content);
  }

  onReady(editor) {
    if (editor.model.schema.isRegistered('image')) {
      editor.model.schema.extend('image', { allowAttributes: 'blockIndent' });
    }
    let token = this.authenticationService.userValue?.jwtToken;
    editor.plugins.get('FileRepository').createUploadAdapter = function (
      loader
    ) {
      return new CkEditorUploadAdapter(loader, token);
    };
  }
}
