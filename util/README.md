# OSS C# SDK Performance Test

## 关于
oss-perf-test-v1.0.0 是OSS C# SDK性能测试的应用示例工具。

## 编译运行
编译运行前请修改 oss-perf-test-v1.0.0 的当前工作目录下的配置文件 oss.ini 中的 Endpoint、AccessKeyId、AccessKeySecret、BucketName 为您的真实信息。 

### 运行
在工程目录下执行 `mono oss-perf-test-v1.0.0.exe` 可以获取帮助信息， 按照  `mono oss-perf-test-v1.0.0.exe -c upload -f mylocalfilename -k myobjectkeyname`  格式修改相应参数可以上传文件。
