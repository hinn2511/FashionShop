import { DialogService } from 'src/app/_services/dialog.service';
import { ToastrService } from 'ngx-toastr';
import { AuthenticationService } from 'src/app/_services/authentication.service';
import { Setting } from './../../_models/setting';
import { SettingService } from './../../_services/setting.service';
import { Component, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-admin-setting',
  templateUrl: './admin-setting.component.html',
  styleUrls: ['./admin-setting.component.css'],
})
export class AdminSettingComponent implements OnInit {
  setting: Setting;
  editType: string = '';
  showUploader: boolean = false;  
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  fileUrl = environment.fileUrl;

  width = 1600;
  height = 900;
  cropOption = 'fill';
  ratio = 0;

  constructor(
    private settingService: SettingService,
    private authenticationService: AuthenticationService,
    private toastr: ToastrService,
    private dialogService: DialogService
  ) {}

  ngOnInit(): void {
    this.loadSetting();
    this.initializeUploader();
  }

  loadSetting() {
    this.settingService.getSettings().subscribe((result) => {
      this.setting = result;
    });
  }

  edit(type: string) {
    this.editType = type;
    let imageIds: number[] = [];
    switch (type) {
      case 'customerLogin': {
        imageIds.push(this.setting.clientLoginPhotoId);
        break;
      }
      case 'customerRegister': {
        imageIds.push(this.setting.clientRegisterPhotoId);
        break;
      }
      default: {
        imageIds.push(this.setting.administratorLoginPhotoId);
        break;
      }
    }
    this.dialogService
      .openPhotoSelectorDialog('Select a photo', false, imageIds)
      .subscribe(
        (result) => {
          if (result.length > 0) {
            let selectedPhoto = result.find((x) => x.isSelected);
            this.updatePhoto(selectedPhoto.id);
          }
        },
        (error) => {
          this.toastr.error(error, 'Error');
        }
      );
  }

  updatePhoto(photoId: number) {
    this.settingService.updateBackground(photoId, this.editType).subscribe(
      (response) => {
        this.toastr.success(response.message, 'Success');
        this.loadSetting();
      },

      (error) => {
        this.toastr.error(error, 'Error');
      }
    );
  }

  uploaderToggle()
  {
    this.showUploader = !this.showUploader;
  }

  initializeUploader() {
    let uploadUrl =
    this.fileUrl + `/image?width=${this.width}&height=${this.height}&ratio=${this.ratio}&cropOption=${this.cropOption}`;
    this.uploader = new FileUploader({
      url: uploadUrl,
      authToken: 'Bearer ' + this.authenticationService.userValue.jwtToken,
      isHTML5: true,
      allowedFileType: ['image', 'video'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 2 * 1024 * 1024,
    });

    this.uploader.onBeforeUploadItem = (file) => {
      if (file.file.type == 'video/mp4')
        this.uploader.setOptions({
          url: uploadUrl.replace('image', 'video'),
        });
    };

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = true;
    };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        this.toastr.success("Photos have been added", "Success");
      }
    };
    this.uploader.onErrorItem  = (item, response, status, headers) => {
      this.toastr.error("Something wrong happen!", "Error")
    }
  }
}
