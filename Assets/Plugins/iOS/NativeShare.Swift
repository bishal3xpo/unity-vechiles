import Foundation
import UIKit

@objc public class NativeShare: NSObject {
    @objc public static func shareFile(_ filePath: String) {
        DispatchQueue.main.async {
            let url = URL(fileURLWithPath: filePath)
            let activityViewController = UIActivityViewController(activityItems: [url], applicationActivities: nil)
            
            if let rootViewController = UIApplication.shared.windows.first?.rootViewController {
                if UIDevice.current.userInterfaceIdiom == .pad {
                    activityViewController.popoverPresentationController?.sourceView = rootViewController.view
                    activityViewController.popoverPresentationController?.sourceRect = CGRect(x: rootViewController.view.bounds.midX, y: rootViewController.view.bounds.midY, width: 0, height: 0)
                    activityViewController.popoverPresentationController?.permittedArrowDirections = []
                }
                rootViewController.present(activityViewController, animated: true, completion: nil)
            }
        }
    }
}
